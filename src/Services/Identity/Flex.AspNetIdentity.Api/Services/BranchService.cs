//using Flex.System.Grpc;
//using Flex.AspNetIdentity.Api.Services.Interfaces;

//namespace Flex.AspNetIdentity.Api.Services
//{
//    public class BranchClientService : IBranchService
//    {
//        private readonly BranchService.BranchServiceClient _branchServiceClient;

//        public BranchClientService(BranchService.BranchServiceClient branchServiceClient)
//        {
//            _branchServiceClient = branchServiceClient;
//        }

//        public async Task<bool> IsBranchExistsAsync(long branchId)
//        {
//            var reply = await _branchServiceClient.CheckBranchExistsAsync(new BranchRequest { BranchId = branchId });
//            var result = reply.Exists;

//            return result;
//        }
//    }
//}
