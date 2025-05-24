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

public class RoleDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public bool IsActive { get; set; }
    public List<ClaimDto> Claims { get; set; } = new();
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
        // ===== Role =====
        Task<PagedResult<RolePagingDto>> GetRolePagedAsync(GetRolesPagingRequest request);
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<RoleDto?> GetByIdAsync(long id);
        Task<RoleDto?> GetBySystemNameAsync(string systemName);

        Task<long> CreateAsync(CreateRoleDto dto);
        Task UpdateAsync(long roleId, UpdateRoleDto dto);
        Task DeleteAsync(long roleId); // soft delete

        // ===== Role Claims =====
        Task<IEnumerable<ClaimDto>> GetClaimsAsync(long roleId);
        Task AddClaimsAsync(long roleId, IEnumerable<ClaimDto> claims);
        Task RemoveClaimAsync(long roleId, ClaimDto claim);

        // ===== Approve RoleRequest → Create Role + Claims =====
        Task<long> CreateFromApprovedRequestAsync(string requestedDataJson); // dùng JSON snapshot
    }
}
