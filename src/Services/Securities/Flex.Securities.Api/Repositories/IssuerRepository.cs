using Microsoft.EntityFrameworkCore;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;
using Flex.Securities.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.Securities;
using Flex.Shared.SeedWork;
using Flex.Infrastructure.EF;
using Flex.Shared.Enums.General;

namespace Flex.Securities.Api.Repositories
{
    public class IssuerRepository : RepositoryBase<CatalogIssuer, long, SecuritiesDbContext>, IIssuerRepository
    {
        public IssuerRepository(SecuritiesDbContext dbContext, IUnitOfWork<SecuritiesDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
        }

        #region Query
        public async Task<PagedResult<CatalogIssuer>> GetPagingIssuersAsync(GetIssuersPagingRequest request)
        {
            // Filter
            var query = this.FindAll()
                .WhereIf(!string.IsNullOrEmpty(request.Name), b => b.Name.ToUpper().Contains(request.Name.ToUpper()))
                .WhereIf(request.Status.HasValue, b => b.Status == request.Status.Value);

            // Paging
            var result = await query.ToPagedResultAsync(request);

            return result;
        }

        public async Task<CatalogIssuer?> GetIssuerByIdAsync(long issuerId)
        {
            return await this.FindByCondition(i => i.Id.Equals(issuerId))
                             .Include(c => c.Securities).FirstOrDefaultAsync();
        }
        #endregion

        #region Command
        public Task CreateIssuerAsync(CatalogIssuer issuer)
        {
            return this.CreateAsync(issuer);
        }

        public Task ApproveIssuerAsync(CatalogIssuer issuer)
        {
            issuer.Status = EEntityStatus.Approved;

            return this.UpdateAsync(issuer);
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
