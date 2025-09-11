using Flex.Contracts.Domains.Interfaces;
using Flex.Shared.SeedWork;
using Flex.System.Api.Entities;
using Flex.System.Api.Entities.Views;
using Flex.System.Api.Models.Branch;
using Flex.System.Api.Persistence;

namespace Flex.System.Api.Repositories.Interfaces
{
    public interface IBranchRepository : IRepositoryBase<Branch, long, SystemDbContext>
    {
        IQueryable<BrandRequestView> GetBranchCombinedQuery();
        Task<Branch?> GetByCodeAsync(string code);
        Task<bool> ExistsByCodeAsync(string code);
    }
}
