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
                // ===== Get branches by codes =====
                var branchesByCode = await _branchService.GetBranchesByCodesAsync(request.Codes);

                // ===== Build response =====
                var response = new BatchGetBranchesResponse();

                // ===== Add found branches =====
                foreach (var kvp in branchesByCode)
                {
                    var branch = new Branch
                    {
                        Code = kvp.Value.Code,
                        Name = kvp.Value.Name
                    };
                    response.BranchesByCode.Add(kvp.Key, branch);
                }

                // ===== Add not found codes =====
                var foundCodes = branchesByCode.Keys.ToHashSet();
                var notFoundCodes = request.Codes.Where(code => !foundCodes.Contains(code)).ToList();
                response.NotFoundCodes.AddRange(notFoundCodes);

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Failed to get branches: {ex.Message}"));
            }
        }
    }
}
