using Flex.Contracts.Domains.Interfaces;
using Flex.System.Api.Persistence;

namespace Flex.System.Api.Repositories.Interfaces
{
    public interface IBranchRequestRepository : IRepositoryBase<Entities.BranchRequest, long, SystemDbContext>
    {
    }
}
