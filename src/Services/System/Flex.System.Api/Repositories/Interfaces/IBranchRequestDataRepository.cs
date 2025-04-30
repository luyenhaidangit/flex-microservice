using Flex.Contracts.Domains.Interfaces;
using Flex.System.Api.Entities;
using Flex.System.Api.Persistence;

namespace Flex.System.Api.Repositories.Interfaces
{
    public interface IBranchRequestDataRepository : IRepositoryBase<BranchRequestData, long, SystemDbContext>
    {
    }
}
