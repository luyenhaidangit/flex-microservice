using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.Contracts.Domains.Interfaces;

namespace Flex.AspNetIdentity.Api.Repositories.Interfaces
{
    public interface IPermissionRepository : IRepositoryBase<Permission, long, IdentityDbContext>
    {
    }
}
