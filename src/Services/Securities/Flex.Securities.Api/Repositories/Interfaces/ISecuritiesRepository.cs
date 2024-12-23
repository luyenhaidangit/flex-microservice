using Flex.Contracts.Domains.Interfaces;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;

namespace Flex.Securities.Api.Repositories.Interfaces
{
    public interface ISecuritiesRepository : IRepositoryBase<CatalogSecurities, long, SecuritiesDbContext>
    {
    }
}
