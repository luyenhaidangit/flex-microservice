using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.Contracts.Domains.Interfaces;
using Flex.System.Api.Entities;

namespace Flex.AspNetIdentity.Api.Repositories.Interfaces
{
    public interface IRoleRequestRepository : IRepositoryBase<RoleRequest, long, IdentityDbContext>
    {
    }
}
