using Flex.Contracts.Domains.Interfaces;
using Flex.System.Api.Entities;
using Flex.System.Api.Persistence;

namespace Flex.System.Api.Repositories.Interfaces
{
    public interface IBranchRequestHeaderRepository : IRepositoryBase<BranchRequestHeader, long, SystemDbContext>
    {
    }
}
