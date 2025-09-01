using Grpc.Core;
using Flex.System.Grpc.Services;
using Flex.System.Api.Services.Interfaces;

namespace Flex.System.Api.Grpc
{
    public class BranchGrpcService : BranchService.BranchServiceBase
    {
        private readonly IBranchService _branchService;

        public BranchGrpcService(IBranchService branchService)
        {
            _branchService = branchService;
        }

        public override async Task<BatchGetBranchesResponse> BatchGetBranches(BatchGetBranchesRequest request, ServerCallContext context)
        {
            try
            {
                // ===== Get branches by ids =====
                var branchesById = await _branchService.GetBranchesByIdsAsync(request.Ids);

                // ===== Build response =====
                var response = new BatchGetBranchesResponse();

                // ===== Add found branches =====
                foreach (var kvp in branchesById)
                {
                    var branch = new Branch
                    {
                        Id = kvp.Value.Id,
                        Name = kvp.Value.Name
                    };
                    response.BranchesById.Add(kvp.Key, branch);
                }

                // ===== Add not found ids =====
                var foundIds = branchesById.Keys.ToHashSet();
                var notFoundIds = request.Ids.Where(id => !foundIds.Contains(id)).ToList();
                response.NotFoundCodes.AddRange(notFoundIds);

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Failed to get branches: {ex.Message}"));
            }
        }
    }
}
