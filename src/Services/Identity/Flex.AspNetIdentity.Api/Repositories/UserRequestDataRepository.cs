using Flex.System.Api.Entities;
using Flex.System.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.Infrastructure.Common.Repositories;
using Flex.Contracts.Domains.Interfaces;

namespace Flex.System.Api.Repositories
{
    public class UserRequestDataRepository : RepositoryBase<UserRequestData, long, IdentityDbContext>, IUserRequestDataRepository
    {
        public UserRequestDataRepository(IdentityDbContext dbContext, IUnitOfWork<IdentityDbContext> unitOfWork) 
            : base(dbContext, unitOfWork)
        {
        }
    }
}
