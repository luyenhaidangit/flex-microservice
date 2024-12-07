using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<CatalogSecurities>> GetSecuritiesAsync() => await FindAll().ToListAsync();

        public Task<CatalogSecurities> GetSecuritiesAsync(long id) => GetByIdAsync(id);

        public Task<CatalogSecurities> GetSecuritiesByNoAsync(string productNo) =>
            FindByCondition(x => x.No.Equals(productNo)).SingleOrDefaultAsync();

        public Task CreateSecuritiesAsync(CatalogSecurities product) => CreateAsync(product);

        public Task UpdateSecuritiesAsync(CatalogSecurities product) => UpdateAsync(product);

        public async Task DeleteSecuritiesAsync(long id)
        {
            var product = await GetSecuritiesAsync(id);
            if (product != null) await DeleteAsync(product);
        }
    }
}
