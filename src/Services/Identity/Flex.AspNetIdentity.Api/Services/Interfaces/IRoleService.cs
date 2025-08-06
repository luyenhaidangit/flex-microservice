using Flex.AspNetIdentity.Api.Models;
using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    public interface IRoleService
    {
        // Query
        Task<PagedResult<RoleApprovedListItemDto>> GetApprovedRolesPagedAsync(GetRolesPagingRequest request);
        Task<RoleDto> GetApprovedRoleByCodeAsync(string code);
        Task<List<RoleChangeHistoryDto>> GetApprovedRoleChangeHistoryAsync(string code);
        Task<long> CreateRoleRequestAsync(CreateRoleRequestDto request);
        Task<long> CreateUpdateRoleRequestAsync(string code, UpdateRoleRequestDto dto);
        Task<long> CreateDeleteRoleRequestAsync(string code, DeleteRoleRequestDto request);
        Task<PagedResult<RolePendingPagingDto>> GetPendingRolesPagedAsync(GetRolesPagingRequest request);
        Task<RoleRequestDetailDto> GetPendingRoleByIdAsync(long requestId);
        Task<List<RoleImpactDto>> GetRoleRequestImpactAsync(long requestId);
        Task<string?> CompareRoleWithRequestAsync(long requestId); 

        // ===== API MỚI =====
        /// <summary>
        /// Lấy danh sách yêu cầu chờ duyệt (Pending/Draft)
        /// </summary>
        Task<PagedResult<RoleRequestDto>> GetPendingRequestsAsync(PendingRequestsPagingRequest request);
        
        /// <summary>
        /// So sánh bản chính và bản nháp
        /// </summary>
        Task<RoleComparisonDto?> GetRoleComparisonAsync(long requestId);
        // Approve
        Task ApproveRoleRequestAsync(long requestId, string? comment = null, string? approvedBy = null);
        Task RejectRoleRequestAsync(long requestId, string reason, string rejectedBy);
        Task CancelRoleRequestAsync(long requestId, string currentUser); // Maker huỷ trước khi duyệt

        // ===== Lịch sử duyệt / audit =====
        //Task<IEnumerable<ApprovalLogDto>> GetRoleRequestApprovalLogAsync(long requestId);

        // ===== (Optional) Claims hoặc phân quyền role =====
        Task<IEnumerable<ClaimDto>> GetClaimsAsync(long roleId);
        Task AddClaimsAsync(long roleId, IEnumerable<ClaimDto> claims);
        Task RemoveClaimAsync(long roleId, ClaimDto claim);

        Task<RoleRequestDto?> GetDraftCreateRequestByCodeAsync(string code, string currentUser);
        /// <summary>
        /// Lấy bản nháp (pending/draft) của một vai trò cụ thể
        /// </summary>
        Task<RoleRequestDto?> GetDraftByRoleAsync(long roleId);
    }
}
