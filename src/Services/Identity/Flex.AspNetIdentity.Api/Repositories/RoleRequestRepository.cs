using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.System.Api.Entities;
using Flex.System.Api.Repositories.Interfaces;

namespace Flex.AspNetIdentity.Api.Repositories
{
    public class RoleRequestRepository : RepositoryBase<RoleRequest, long, IdentityDbContext>, IRoleRequestRepository
    {
        public RoleRequestRepository(IdentityDbContext dbContext, IUnitOfWork<IdentityDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
        }
    }
}
