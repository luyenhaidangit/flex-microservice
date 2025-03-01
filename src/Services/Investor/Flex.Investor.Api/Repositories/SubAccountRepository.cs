using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Investor.Api.Entities;
using Flex.Investor.Api.Persistence;
using Flex.Investor.Api.Repositories.Interfaces;

namespace Flex.Investor.Api.Repositories
{
    public class SubAccountRepository : RepositoryBase<SubAccount, long, InvestorDbContext>, ISubAccountRepository
    {
        public SubAccountRepository(InvestorDbContext dbContext, IUnitOfWork<InvestorDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
        }
    }
}
