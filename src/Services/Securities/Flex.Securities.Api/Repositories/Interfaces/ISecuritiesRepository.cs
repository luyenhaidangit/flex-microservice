using Flex.Contracts.Domains.Interfaces;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;

namespace Flex.Securities.Api.Repositories.Interfaces
{
    public interface ISecuritiesRepository : IRepositoryBase<CatalogSecurities, long, SecuritiesDbContext>
    {
        // Query
        Task<List<CatalogSecurities>> GetSecuritiesByIssuerAsync(long issuerNo);
        Task<CatalogSecurities?> GetSecuritiesByIdAsync(long securitiesNo);

        // Command
        Task CreateSecuritiesAsync(CatalogSecurities securities);
        Task UpdateSecuritiesAsync(CatalogSecurities securities);
        Task DeleteSecuritiesAsync(long securitiesNo);
    }
}
