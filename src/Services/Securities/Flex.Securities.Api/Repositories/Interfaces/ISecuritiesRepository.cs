using Flex.Contracts.Domains.Interfaces;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;

namespace Flex.Securities.Api.Repositories.Interfaces
{
    public interface ISecuritiesRepository : IRepositoryBase<CatalogSecurity, long, SecuritiesDbContext>
    {
        // Query
        Task<List<CatalogSecurity>> GetSecuritiesByIssuerAsync(long issuerNo);
        Task<CatalogSecurity?> GetSecuritiesByIdAsync(long securitiesNo);

        // Command
        Task CreateSecuritiesAsync(CatalogSecurity securities);
        Task UpdateSecuritiesAsync(CatalogSecurity securities);
        Task DeleteSecuritiesAsync(long securitiesNo);
    }
}
