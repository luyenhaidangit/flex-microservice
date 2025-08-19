using Flex.System.Api.Models;
using Flex.Shared.SeedWork;
using Flex.System.Api.Models.Branch;

namespace Flex.System.Api.Services.Interfaces
{
    public interface IBranchService
    {
        // Query
        Task<PagedResult<BranchApprovedItemDto>> GetApprovedBranchesPagedAsync(GetBranchPagingRequest request);
        Task<BranchDto> GetApprovedBranchByCodeAsync(string code);
        Task<List<BranchChangeHistoryDto>> GetApprovedBranchChangeHistoryAsync(string code);
        
        // Command
        Task<long> CreateBranchRequestAsync(CreateBranchRequestDto request);
        Task<long> CreateUpdateBranchRequestAsync(string code, UpdateBranchRequestDto dto);
        Task<long> CreateDeleteBranchRequestAsync(string code, DeleteBranchRequestDto request);
        
        // Pending Management
        Task<PagedResult<BranchPendingPagingDto>> GetPendingBranchesPagedAsync(GetBranchPagingRequest request);
        Task<BranchRequestDetailDto> GetPendingBranchByIdAsync(long requestId);
        Task<BranchApprovalResultDto> ApprovePendingBranchRequestAsync(long requestId, string? comment = null);
        Task<BranchApprovalResultDto> RejectPendingBranchRequestAsync(long requestId, string? reason = null);
    }
}
