using Flex.Contracts.Domains.Interfaces;
using Flex.Investor.Api.Persistence;
using Flex.Investor.Api.Entities;

namespace Flex.Investor.Api.Repositories.Interfaces
{
    public interface ISubAccountRepository : IRepositoryBase<SubAccount, long, InvestorDbContext>
    {
    }
}
