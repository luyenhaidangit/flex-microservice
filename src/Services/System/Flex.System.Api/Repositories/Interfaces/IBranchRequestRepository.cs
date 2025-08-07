using Flex.System.Api.Entities;
using Flex.System.Api.Models;
using Flex.Shared.SeedWork;
using Flex.Contracts.Domains.Interfaces;
using Flex.System.Api.Persistence;

namespace Flex.System.Api.Repositories.Interfaces
{
    public interface IBranchRequestRepository : IRepositoryBase<BranchRequest, long, SystemDbContext>
    {
        Task<PagedResult<BranchPendingPagingDto>> GetPendingPagedAsync(GetBranchPagingRequest request);
        Task<BranchRequest?> GetPendingByIdAsync(long requestId);
    }
}
