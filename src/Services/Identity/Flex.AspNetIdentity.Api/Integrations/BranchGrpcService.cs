using Flex.AspNetIdentity.Api.Integrations.Interfaces;
using Flex.AspNetIdentity.Api.Models;
using Flex.System.Grpc.Services;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Flex.AspNetIdentity.Api.Integrations
{
    /// <summary>
    /// Branch Integration Service - tích hợp với System service qua gRPC
    /// </summary>
    public class BranchGrpcService : IBranchIntegrationService
    {
        private readonly BranchService.BranchServiceClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BranchGrpcService> _logger;
        private readonly ConcurrentDictionary<string, BranchDto> _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public BranchGrpcService(
            BranchService.BranchServiceClient client,
            IConfiguration configuration,
            ILogger<BranchGrpcService> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = new ConcurrentDictionary<string, BranchDto>();
        }

        /// <summary>
        /// Lấy danh sách branches theo danh sách mã codes
        /// </summary>
        public async Task<IReadOnlyList<BranchDto>> BatchGetBranchesAsync(IEnumerable<string> codes, CancellationToken ct = default)
        {
            if (codes == null || !codes.Any())
            {
                _logger.LogWarning("BatchGetBranchesAsync called with empty or null codes");
                return Array.Empty<BranchDto>();
            }

            var codeList = codes.ToList();
            var request = new BatchGetBranchesRequest();
            request.Codes.AddRange(codeList);

            try
            {
                _logger.LogDebug("Calling gRPC BatchGetBranches with {Count} codes", codeList.Count);

                var response = await _client.BatchGetBranchesAsync(
                    request,
                    cancellationToken: ct);

                var result = response.BranchesByCode
                    .Select(kvp => new BranchDto(kvp.Key, kvp.Value.Name))
                    .ToList();

                // Cache results
                //foreach (var branch in result)
                //{
                //    _cache.TryAdd(branch.Code, branch);
                //}

                // Log not found codes
                if (response.NotFoundCodes.Any())
                {
                    _logger.LogWarning("Branches not found: {NotFoundCodes}", string.Join(", ", response.NotFoundCodes));
                }

                _logger.LogInformation("Successfully retrieved {FoundCount} branches out of {TotalCount} requested", 
                    result.Count, codeList.Count);

                return result;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning("No branches found for codes: {Codes}", string.Join(", ", codeList));
                return Array.Empty<BranchDto>();
            }
            catch (RpcException ex) when (ex.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded)
            {
                _logger.LogError(ex, "Transient error calling System service for codes: {Codes}", string.Join(", ", codeList));
                throw new TransientException($"System service transient error: {ex.Status.Detail}", ex);
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error calling System service for codes: {Codes}. Status: {StatusCode}, Detail: {Detail}", 
                    string.Join(", ", codeList), ex.Status.StatusCode, ex.Status.Detail);
                throw new InvalidOperationException($"Failed to retrieve branches: {ex.Status.Detail}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling System service for codes: {Codes}", string.Join(", ", codeList));
                throw;
            }
        }

        /// <summary>
        /// Lấy thông tin một branch theo mã
        /// </summary>
        public async Task<BranchDto?> GetBranchByCodeAsync(string code, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("GetBranchByCodeAsync called with null or empty code");
                return null;
            }

            // Check cache first
            if (_cache.TryGetValue(code, out var cachedBranch))
            {
                _logger.LogDebug("Branch {Code} found in cache", code);
                return cachedBranch;
            }

            try
            {
                var branches = await BatchGetBranchesAsync(new[] { code }, ct);
                return branches.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving branch {Code}", code);
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra xem branch có tồn tại không
        /// </summary>
        public async Task<bool> BranchExistsAsync(string code, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            try
            {
                var branch = await GetBranchByCodeAsync(code, ct);
                return branch != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of branch {Code}", code);
                return false;
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả branches (nếu có endpoint)
        /// </summary>
        public async Task<IReadOnlyList<BranchDto>> GetAllBranchesAsync(CancellationToken ct = default)
        {
            // Note: This method would require an additional gRPC endpoint in BranchService
            // For now, we'll return an empty list and log that it's not implemented
            _logger.LogWarning("GetAllBranchesAsync is not implemented - requires additional gRPC endpoint");
            
            // TODO: Implement when GetAllBranches gRPC endpoint is available
            // var request = new GetAllBranchesRequest();
            // var response = await _client.GetAllBranchesAsync(request, deadline: DateTime.UtcNow.AddSeconds(30), cancellationToken: ct);
            // return response.Branches.Select(b => new BranchDto(b.Code, b.Name)).ToList();
            
            return Array.Empty<BranchDto>();
        }

        /// <summary>
        /// Clear cache (useful for testing or when cache becomes stale)
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
            _logger.LogInformation("Branch cache cleared");
        }

        /// <summary>
        /// Get cache statistics
        /// </summary>
        public (int Count, int Capacity) GetCacheStats()
        {
            return (_cache.Count, _cache.Count); // Capacity is not directly available in ConcurrentDictionary
        }
    }
}
