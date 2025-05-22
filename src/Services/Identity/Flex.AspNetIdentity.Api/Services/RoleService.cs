using Flex.AspNetIdentity.Api.Services.Interfaces;

namespace Flex.AspNetIdentity.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;

        public RoleService(ILogger<RoleService> logger)
        {
            _logger = logger;
        }

        public Task AddClaimsAsync(long roleId, IEnumerable<ClaimDto> claims)
        {
            throw new NotImplementedException();
        }

        public Task<long> CreateAsync(CreateRoleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<long> CreateFromApprovedRequestAsync(string requestedDataJson)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(long roleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RoleDto?> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<RoleDto?> GetBySystemNameAsync(string systemName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ClaimDto>> GetClaimsAsync(long roleId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(long roleId, ClaimDto claim)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(long roleId, UpdateRoleDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
