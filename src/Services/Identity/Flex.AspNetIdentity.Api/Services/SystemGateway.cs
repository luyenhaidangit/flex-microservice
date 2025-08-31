using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.System.Grpc.Services;
using Grpc.Core;
using Microsoft.Extensions.Configuration;

namespace Flex.AspNetIdentity.Api.Services
{
    public sealed class SystemGateway : ISystemGateway
    {
        private readonly BranchService.BranchServiceClient client;
        private readonly IConfiguration config;

        public SystemGateway(
            BranchService.BranchServiceClient client,
            IConfiguration config)
        {
            this.client = client;
            this.config = config;
        }

        public async Task<IReadOnlyList<BranchDto>> BatchGetBranchesAsync(IEnumerable<string> codes, CancellationToken ct)
        {
            var deadlineSec = config.GetValue("Grpc:SystemService:DeadlineSeconds", 3);
            var request = new BatchGetBranchesRequest();
            request.Codes.AddRange(codes);

            try
            {
                var response = await client.BatchGetBranchesAsync(
                    request,
                    deadline: DateTime.UtcNow.AddSeconds(deadlineSec),
                    cancellationToken: ct);

                return response.BranchesByCode.Select(kvp => new BranchDto(kvp.Key, kvp.Value.Name)).ToList();
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return Array.Empty<BranchDto>();
            }
            catch (RpcException ex) when (ex.StatusCode is StatusCode.Unavailable or StatusCode.DeadlineExceeded)
            {
                // Cho phép upper-layer retry hoặc fallback
                throw new TransientException($"SystemService transient error: {ex.Status.Detail}", ex);
            }
        }
    }
}

