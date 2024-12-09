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

        #region Query
        public Task<List<CatalogSecurities>> GetSecuritiesByIssuerAsync(string issuerNo)
        {
            return this.FindByCondition(x => x.IssuerNo.Equals(issuerNo)).ToListAsync();
        }

        public Task<CatalogSecurities?> GetSecuritiesByNoAsync(string securitiesNo)
        {
            return this.FindByCondition(x => x.No.Equals(securitiesNo)).SingleOrDefaultAsync();
        }

        public async Task<string> GenerateSecuritiesNo()
        {
            var maxCode = await this.FindByCondition(s => !s.No.StartsWith("9"))
                                    .Select(s => int.Parse(s.No))
                                    .DefaultIfEmpty(0)
                                    .MaxAsync();

            var newCode = (maxCode + 2).ToString().PadLeft(6, '0');

            return newCode;
        }
        #endregion

        #region Command
        public Task CreateSecuritiesAsync(CatalogSecurities securities) 
        {
            return this.CreateAsync(securities);
        }

        public Task UpdateSecuritiesAsync(CatalogSecurities securities)
        {
            return this.UpdateAsync(securities);
        }

        public async Task DeleteSecuritiesAsync(string securitiesNo)
        {
            var securities = await this.GetSecuritiesByNoAsync(securitiesNo);

            if (securities is not null)
            {
                await DeleteAsync(securities);
            }
        }
        #endregion
    }
}
