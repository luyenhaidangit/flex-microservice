using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.System.Api.Persistence;
using Flex.System.Api.Repositories.Interfaces;

namespace Flex.System.Api.Repositories
{
    public class BranchRequestRepository : RepositoryBase<Entities.BranchRequest, long, SystemDbContext>, IBranchRequestRepository
    {
        public BranchRequestRepository(SystemDbContext dbContext, IUnitOfWork<SystemDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
        }
    }
}
