using Microsoft.EntityFrameworkCore;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;
using Flex.Securities.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.Securities;
using Flex.Shared.SeedWork;
using Flex.Infrastructure.EF;

namespace Flex.Securities.Api.Repositories
{
    public class IssuerRepository : RepositoryBase<CatalogIssuer, long, SecuritiesDbContext>, IIssuerRepository
    {
        public IssuerRepository(SecuritiesDbContext dbContext, IUnitOfWork<SecuritiesDbContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }

        #region Query
        public async Task<PagedResult<CatalogIssuer>> GetPagingIssuersAsync(GetIssuersPagingRequest request)
        {
            // Filter
            var query = this.FindAll()
                .WhereIf(!string.IsNullOrEmpty(request.Name), b => b.Name.ToUpper().Contains(request.Name.ToUpper()));
                //.WhereIf(request.Status.HasValue, b => b.Status == request.Status.Value);

            // Paging
            var result = await query.ToPagedResultAsync(request);

            return result;
        }
        #endregion
    }
}
