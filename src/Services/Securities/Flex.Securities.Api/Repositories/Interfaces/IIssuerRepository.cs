using Flex.Contracts.Domains.Interfaces;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;

namespace Flex.Securities.Api.Repositories.Interfaces
{
    public interface IIssuerRepository : IRepositoryBase<CatalogIssuer, long, SecuritiesDbContext>
    {
    }
}
