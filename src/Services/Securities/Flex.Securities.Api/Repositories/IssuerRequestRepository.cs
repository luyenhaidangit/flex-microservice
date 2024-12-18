using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Persistence;
using Flex.Securities.Api.Repositories.Interfaces;
using Flex.Shared.Enums.General;
using Newtonsoft.Json;

namespace Flex.Securities.Api.Repositories
{
    public class IssuerRequestRepository : RepositoryBase<CatalogIssuerRequest, long, SecuritiesDbContext>, IIssuerRequestRepository
    {
        public IssuerRequestRepository(SecuritiesDbContext dbContext, IUnitOfWork<SecuritiesDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
        }

        //public Task CreateIssuerRequestAsync(CatalogIssuer issuer)
        //{
        //    var dataProposed = JsonConvert.SerializeObject(issuer);

        //    var request = CatalogIssuerRequest.Create(dataProposed,ERequestType.ADD,ERequestStatus.DRAFT);

        //    return this.CreateAsync(request);
        //}
    }
}
