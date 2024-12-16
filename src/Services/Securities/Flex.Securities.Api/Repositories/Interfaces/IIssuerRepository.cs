using Flex.Contracts.Domains.Interfaces;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;
using Flex.Shared.DTOs.Securities;
using Flex.Shared.SeedWork;

namespace Flex.Securities.Api.Repositories.Interfaces
{
    public interface IIssuerRepository : IRepositoryBase<CatalogIssuer, long, SecuritiesDbContext>
    {
        // Query
        Task<PagedResult<CatalogIssuer>> GetPagingIssuersAsync(GetIssuersPagingRequest request);
        Task<CatalogIssuer?> GetIssuerByIdAsync(long issuerId);

        // Command
        Task CreateIssuerAsync(CatalogIssuer issuer);
        Task UpdateIssuerAsync(CatalogIssuer issuer);
        Task DeleteIssuerAsync(long issuerId);
    }
}
