using Flex.Contracts.Domains.Interfaces;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;

namespace Flex.Securities.Api.Repositories.Interfaces
{
    public interface ISecuritiesRepository : IRepositoryBase<CatalogSecurities, long, SecuritiesDbContext>
    {
        Task<IEnumerable<CatalogSecurities>> GetSecuritiesAsync();
        Task<CatalogSecurities> GetSecuritiesAsync(long id);
        Task<CatalogSecurities> GetSecuritiesByNoAsync(string productNo);
        Task CreateSecuritiesAsync(CatalogSecurities product);
        Task UpdateSecuritiesAsync(CatalogSecurities product);
        Task DeleteSecuritiesAsync(long id);
    }
}
