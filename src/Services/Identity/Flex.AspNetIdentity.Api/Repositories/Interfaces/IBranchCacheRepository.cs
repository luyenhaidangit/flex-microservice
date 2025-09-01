using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;

namespace Flex.AspNetIdentity.Api.Repositories.Interfaces
{
    public interface IBranchCacheRepository : IRepositoryBase<BranchCache, long, IdentityDbContext>
    {
        Task<BranchCache?> GetByCodeAsync(string code);
        Task<List<BranchCache>> GetActiveBranchesAsync();
        Task<bool> ExistsByCodeAsync(string code);
        Task<BranchCache?> GetByIdAsync(long id);
    }
}
