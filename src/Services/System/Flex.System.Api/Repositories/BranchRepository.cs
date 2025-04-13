using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.System.Api.Persistence;
using Flex.System.Api.Repositories.Interfaces;

namespace Flex.System.Api.Repositories
{
    public class BranchRepository : RepositoryBase<Entities.Branch, long, SystemDbContext>, IBranchRepository
    {
        public BranchRepository(SystemDbContext dbContext, IUnitOfWork<SystemDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
        }

        public Task<bool> HasAccounts(long branchId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasTransactions(long branchId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasUsers(long branchId)
        {
            throw new NotImplementedException();
        }
    }
}
