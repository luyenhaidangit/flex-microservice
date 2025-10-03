using Flex.AspNetIdentity.Api.Models.Permission;
using Flex.AspNetIdentity.Api.Models.Role;
using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    /// <summary>
    /// RULE:
    /// Tạo role không được trùng tên nhau.
    /// </summary>
    public interface IRoleService
    {
        Task<PagedResult<RoleApprovedListItemDto>> GetApprovedRolesPagedAsync(GetApproveRolesPagingRequest request);
        Task<RoleDto> GetApprovedRoleByCodeAsync(string code, bool includeClaims = true, bool includeTree = true, string? search = null, CancellationToken ct = default);
        Task<List<RoleChangeHistoryDto>> GetApprovedRoleChangeHistoryAsync(string code);
        Task<long> CreateRoleRequestAsync(CreateRoleRequestDto request);
        Task<long> CreateUpdateRoleRequestAsync(string code, UpdateRoleRequestDto dto);
        Task<long> CreateDeleteRoleRequestAsync(string code, DeleteRoleRequestDto request);
        Task<PagedResult<RolePendingPagingDto>> GetPendingRolesPagedAsync(GetApproveRolesPagingRequest request);
        Task<RoleRequestDetailDto> GetPendingRoleByIdAsync(long requestId);
        Task<RoleApprovalResultDto> ApprovePendingRoleRequestAsync(long requestId, string? comment = null);
        Task<RoleApprovalResultDto> RejectPendingRoleRequestAsync(long requestId, string? reason = null);
        Task<PermissionFlagsResult> GetPermissionFlagsAsync(string? roleCode, CancellationToken ct = default);
        Task UpdateRolePermissionsAsync(string roleCode, IEnumerable<string> permissionCodes, CancellationToken ct = default);
    }
}
