using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Models.Permission;
using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    public interface IRoleService
    {
        Task<PagedResult<RoleApprovedListItemDto>> GetApprovedRolesPagedAsync(GetRolesPagingRequest request);
        Task<RoleDto> GetApprovedRoleByCodeAsync(string code);
        Task<List<RoleChangeHistoryDto>> GetApprovedRoleChangeHistoryAsync(string code);
        Task<long> CreateRoleRequestAsync(CreateRoleRequestDto request);
        Task<long> CreateUpdateRoleRequestAsync(string code, UpdateRoleRequestDto dto);
        Task<long> CreateDeleteRoleRequestAsync(string code, DeleteRoleRequestDto request);
        Task<PagedResult<RolePendingPagingDto>> GetPendingRolesPagedAsync(GetRolesPagingRequest request);
        Task<RoleRequestDetailDto> GetPendingRoleByIdAsync(long requestId);
        Task<RoleApprovalResultDto> ApprovePendingRoleRequestAsync(long requestId, string? comment = null);
        Task<RoleApprovalResultDto> RejectPendingRoleRequestAsync(long requestId, string? reason = null);
        Task<(List<PermissionNodeDto> Root, int Total, int Assignable, int Checked)> GetPermissionFlagsAsync(string roleCode, string? search = null, CancellationToken ct = default);
        Task UpdateRolePermissionsAsync(string roleCode, IEnumerable<string> permissionCodes, CancellationToken ct = default);
    }
}
