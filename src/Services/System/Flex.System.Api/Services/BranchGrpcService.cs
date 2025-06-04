//using Flex.System.Api.Repositories.Interfaces;
//using Flex.System.Grpc;
//using Grpc.Core;
//using Microsoft.EntityFrameworkCore;

//namespace Flex.System.Api.Services
//{
//    public class BranchGrpcService : BranchService.BranchServiceBase
//    {
//        private readonly IBranchMasterRepository _branchMasterRepository;

//        public BranchGrpcService(IBranchMasterRepository branchMasterRepository)
//        {
//            _branchMasterRepository = branchMasterRepository;
//        }

//        public override async Task<BranchReply> CheckBranchExists(BranchRequest request, ServerCallContext context)
//        {
//            var exists = await _branchMasterRepository.FindByCondition(x => x.Id == request.BranchId).AnyAsync();

//            return new BranchReply
//            {
//                Exists = exists
//            };
//        }
//    }
//}
