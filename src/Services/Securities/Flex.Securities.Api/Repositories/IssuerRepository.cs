using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;
using Flex.Securities.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Securities.Api.Repositories
{
    public class IssuerRepository : RepositoryBase<CatalogIssuer, long, SecuritiesDbContext>, IIssuerRepository
    {
        public IssuerRepository(SecuritiesDbContext dbContext, IUnitOfWork<SecuritiesDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
        }

        #region Query
        public async Task<List<CatalogIssuer>> GetAllIssuersAsync()
        {
            return await this.FindAll().ToListAsync();
        }

        public async Task<CatalogIssuer?> GetIssuerByIdAsync(long issuerId)
        {
            return await this.FindByCondition(i => i.Id.Equals(issuerId)).SingleOrDefaultAsync();
        }
        #endregion

        #region Command
        public Task CreateIssuerAsync(CatalogIssuer issuer)
        {
            return this.CreateAsync(issuer);
        }

        public Task UpdateIssuerAsync(CatalogIssuer issuer)
        {
            return this.UpdateAsync(issuer);
        }

        public async Task DeleteIssuerAsync(long issuerId)
        {
            var issuer = await GetIssuerByIdAsync(issuerId);
            if (issuer != null)
            {
                await DeleteAsync(issuer);
            }
        }
        #endregion
    }
}
