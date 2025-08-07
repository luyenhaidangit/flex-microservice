using Flex.Contracts.Domains.Interfaces;
using Flex.System.Api.Entities;
using Flex.System.Api.Models;
using Flex.Shared.SeedWork;
using Flex.System.Api.Persistence;

namespace Flex.System.Api.Repositories.Interfaces
{
    public interface IBranchRepository : IRepositoryBase<Branch, long, SystemDbContext>
    {
        Task<Branch?> GetByCodeAsync(string code);
        Task<bool> ExistsByCodeAsync(string code);
        Task<PagedResult<BranchListItemDto>> GetPagedAsync(GetBranchPagingRequest request);
        Task<PagedResult<BranchListItemDto>> GetApprovedPagedAsync(GetBranchPagingRequest request);
    }
}
