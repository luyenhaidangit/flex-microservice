using Microsoft.EntityFrameworkCore;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;
using Flex.Securities.Api.Repositories.Interfaces;

namespace Flex.Securities.Api.Repositories
{
    public class SecuritiesRepository : RepositoryBase<CatalogSecurity, long, SecuritiesDbContext>, ISecuritiesRepository
    {
        public SecuritiesRepository(SecuritiesDbContext dbContext, IUnitOfWork<SecuritiesDbContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }

        #region Query
        public Task<List<CatalogSecurity>> GetSecuritiesByIssuerAsync(long issuerId)
        {
            return this.FindByCondition(x => x.IssuerId.Equals(issuerId)).ToListAsync();
        }

        public Task<CatalogSecurity?> GetSecuritiesByIdAsync(long securitiesId)
        {
            return this.FindByCondition(x => x.Id.Equals(securitiesId)).SingleOrDefaultAsync();
        }
        #endregion

        #region Command
        public Task CreateSecuritiesAsync(CatalogSecurity securities) 
        {
            return this.CreateAsync(securities);
        }

        public Task UpdateSecuritiesAsync(CatalogSecurity securities)
        {
            return this.UpdateAsync(securities);
        }

        public async Task DeleteSecuritiesAsync(long securitiesNo)
        {
            var securities = await this.GetSecuritiesByIdAsync(securitiesNo);

            if (securities is not null)
            {
                await DeleteAsync(securities);
            }
        }
        #endregion
    }
}
