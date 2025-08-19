using Flex.System.Api.Entities;
using Flex.System.Api.Models;
using Flex.Shared.SeedWork;
using Flex.Contracts.Domains.Interfaces;
using Flex.System.Api.Persistence;
using Flex.System.Api.Models.Branch;

namespace Flex.System.Api.Repositories.Interfaces
{
    public interface IBranchRequestRepository : IRepositoryBase<BranchRequest, long, SystemDbContext>
    {
        Task<BranchRequest?> GetPendingByIdAsync(long requestId);
    }
}
