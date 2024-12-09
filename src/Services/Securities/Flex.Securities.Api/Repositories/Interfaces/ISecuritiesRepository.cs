using Flex.Contracts.Domains.Interfaces;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;

namespace Flex.Securities.Api.Repositories.Interfaces
{
    public interface ISecuritiesRepository : IRepositoryBase<CatalogSecurities, long, SecuritiesDbContext>
    {
        // Query
        Task<List<CatalogSecurities>> GetSecuritiesByIssuerAsync(string issuerNo);
        Task<CatalogSecurities?> GetSecuritiesByNoAsync(string securitiesNo);
        Task<string> GenerateSecuritiesNo();

        // Command
        Task CreateSecuritiesAsync(CatalogSecurities securities);
        Task UpdateSecuritiesAsync(CatalogSecurities securities);
        Task DeleteSecuritiesAsync(string securitiesNo);
    }
}
