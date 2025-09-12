using Flex.AspNetIdentity.Api.Integrations.Interfaces;
using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Models.Branch;
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
        private readonly ConcurrentDictionary<long, BranchLookupDto> _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public BranchGrpcService(
            BranchService.BranchServiceClient client,
            IConfiguration configuration,
            ILogger<BranchGrpcService> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = new ConcurrentDictionary<long, BranchLookupDto>();
        }

        /// <summary>
        /// Lấy danh sách branches theo danh sách ids
        /// </summary>
        public async Task<IReadOnlyList<BranchLookupDto>> BatchGetBranchesAsync(IEnumerable<long> ids, CancellationToken ct = default)
        {
            if (ids == null || !ids.Any())
            {
                _logger.LogWarning("BatchGetBranchesAsync called with empty or null ids");
                return Array.Empty<BranchLookupDto>();
            }

            var idList = ids.ToList();
            
            // Use ids directly (no conversion needed)
            var request = new BatchGetBranchesRequest();
            request.Ids.AddRange(idList);

            try
            {
                _logger.LogDebug("Calling gRPC BatchGetBranches with {Count} ids", idList.Count);

                var response = await _client.BatchGetBranchesAsync(
                    request,
                    cancellationToken: ct);

                var result = new List<BranchLookupDto>();
                
                // Map from ids to BranchDto
                foreach (var kvp in response.BranchesById)
                {
                    var branch = new BranchLookupDto(kvp.Key, kvp.Value.Name);
                    result.Add(branch);
                    
                    // Cache by id
                    _cache.TryAdd(kvp.Key, branch);
                }

                // Log not found ids
                if (response.NotFoundCodes.Any())
                {
                    _logger.LogWarning("Branches not found: {NotFoundIds}", string.Join(", ", response.NotFoundCodes));
                }

                _logger.LogInformation("Successfully retrieved {FoundCount} branches out of {TotalCount} requested", 
                    result.Count, idList.Count);

                return result;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning("No branches found for ids: {Ids}", string.Join(", ", idList));
                return Array.Empty<BranchLookupDto>();
            }
            catch (RpcException ex) when (ex.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded)
            {
                _logger.LogError(ex, "Transient error calling System service for ids: {Ids}", string.Join(", ", idList));
                throw new TransientException($"System service transient error: {ex.Status.Detail}", ex);
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error calling System service for ids: {Ids}. Status: {StatusCode}, Detail: {Detail}", 
                    string.Join(", ", idList), ex.Status.StatusCode, ex.Status.Detail);
                throw new InvalidOperationException($"Failed to retrieve branches: {ex.Status.Detail}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling System service for ids: {Ids}", string.Join(", ", idList));
                throw;
            }
        }

        /// <summary>
        /// Lấy thông tin một branch theo id
        /// </summary>
        public async Task<BranchLookupDto?> GetBranchByIdAsync(long id, CancellationToken ct = default)
        {
            if (id <= 0)
            {
                _logger.LogWarning("GetBranchByIdAsync called with invalid id: {Id}", id);
                return null;
            }

            // Check cache first
            if (_cache.TryGetValue(id, out var cachedBranch))
            {
                _logger.LogDebug("Branch {Id} found in cache", id);
                return cachedBranch;
            }

            try
            {
                var branches = await BatchGetBranchesAsync(new[] { id }, ct);
                return branches.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving branch {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra xem branch có tồn tại không
        /// </summary>
        public async Task<bool> BranchExistsAsync(long id, CancellationToken ct = default)
        {
            if (id <= 0)
            {
                return false;
            }

            try
            {
                var branch = await GetBranchByIdAsync(id, ct);
                return branch != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of branch {Id}", id);
                return false;
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả branches (nếu có endpoint)
        /// </summary>
        public async Task<IReadOnlyList<BranchLookupDto>> GetAllBranchesAsync(CancellationToken ct = default)
        {
            // Note: This method would require an additional gRPC endpoint in BranchService
            // For now, we'll return an empty list and log that it's not implemented
            _logger.LogWarning("GetAllBranchesAsync is not implemented - requires additional gRPC endpoint");
            
            // TODO: Implement when GetAllBranches gRPC endpoint is available
            // var request = new GetAllBranchesRequest();
            // var response = await _client.GetAllBranchesAsync(request, deadline: DateTime.UtcNow.AddSeconds(30), cancellationToken: ct);
            // return response.Branches.Select(b => new BranchDto(b.Code, b.Name)).ToList();
            
            return Array.Empty<BranchLookupDto>();
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
