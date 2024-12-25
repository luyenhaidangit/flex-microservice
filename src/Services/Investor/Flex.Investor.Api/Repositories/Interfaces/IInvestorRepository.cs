using Flex.Contracts.Domains.Interfaces;
using Flex.Investor.Api.Persistence;

namespace Flex.Investor.Api.Repositories.Interfaces
{
    public interface IInvestorRepository : IRepositoryBase<Entities.Investor, long, InvestorDbContext>
    {
        IQueryable<Entities.Investor> GetSampleData();
    }
}
