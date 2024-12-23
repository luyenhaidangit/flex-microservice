using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;
using Flex.Securities.Api.Repositories.Interfaces;

namespace Flex.Securities.Api.Repositories
{
    public class SecuritiesRepository : RepositoryBase<CatalogSecurities, long, SecuritiesDbContext>, ISecuritiesRepository
    {
        public SecuritiesRepository(SecuritiesDbContext dbContext, IUnitOfWork<SecuritiesDbContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }
    }
}
