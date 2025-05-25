using Flex.AspNetIdentity.Api.Models;
using Flex.Shared.SeedWork;

public class CreateRoleDto
{
    public string Name { get; set; }
    public string Code { get; set; }
    public List<string>? Claims { get; set; }
}

public class UpdateRoleDto
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public List<string>? Claims { get; set; }
}

public class ClaimDto
{
    public string Type { get; set; } = "permission";
    public string Value { get; set; } = default!;
}

namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    public interface IRoleService
    {
        // ===== Role gốc (đã duyệt) =====
        Task<PagedResult<RolePagingDto>> GetRolePagedAsync(GetRolesPagingRequest request);
        Task<RoleDto?> GetRoleByIdAsync(long id);
        //Task<IEnumerable<RoleChangeLogDto>> GetRoleChangeHistoryAsync(long roleId);

        // ===== Yêu cầu (Request) =====
        //Task<RoleRequestDto?> GetRoleRequestByIdAsync(long requestId);
        //Task<List<RoleImpactDto>> GetRoleRequestImpactAsync(long requestId); // ảnh hưởng nếu duyệt
        Task<string?> CompareRoleWithRequestAsync(long requestId); // trả về diff dạng HTML/json (tuỳ UI)
        Task<long> CreateAddRoleRequestAsync(CreateRoleDto dto);
        Task<long> CreateUpdateRoleRequestAsync(long roleId, UpdateRoleDto dto);
        Task<long> CreateDeleteRoleRequestAsync(long roleId);

        // ===== Phê duyệt yêu cầu =====
        Task ApproveRoleRequestAsync(long requestId, string? comment = null);
        Task RejectRoleRequestAsync(long requestId, string reason);
        Task CancelRoleRequestAsync(long requestId); // Maker huỷ trước khi duyệt

        // ===== Lịch sử duyệt / audit =====
        //Task<IEnumerable<ApprovalLogDto>> GetRoleRequestApprovalLogAsync(long requestId);

        // ===== (Optional) Claims hoặc phân quyền role =====
        Task<IEnumerable<ClaimDto>> GetClaimsAsync(long roleId);
        Task AddClaimsAsync(long roleId, IEnumerable<ClaimDto> claims);
        Task RemoveClaimAsync(long roleId, ClaimDto claim);
    }
}
