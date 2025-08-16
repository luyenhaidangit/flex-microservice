using Flex.AspNetIdentity.Api.Entities;
using Flex.Infrastructure.Common.Repositories;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.Contracts.Domains.Interfaces;

namespace Flex.AspNetIdentity.Api.Repositories
{
    public class PermissionRepository : RepositoryBase<Permission, long, IdentityDbContext>, IPermissionRepository
    {
        public PermissionRepository(IdentityDbContext context, IUnitOfWork<IdentityDbContext> unitOfWork)
            : base(context, unitOfWork)
        {
        }
    }
}
