using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Investor.Api.Persistence;
using Flex.Investor.Api.Repositories.Interfaces;

namespace Flex.Investor.Api.Repositories
{
    public class InvestorRepository : RepositoryBase<Entities.Investor, long, InvestorDbContext>, IInvestorRepository
    {
        public InvestorRepository(InvestorDbContext dbContext, IUnitOfWork<InvestorDbContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }
    }
}
