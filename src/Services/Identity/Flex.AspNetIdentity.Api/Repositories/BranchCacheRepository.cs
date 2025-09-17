using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flex.AspNetIdentity.Api.Repositories
{
    public class BranchCacheRepository : RepositoryBase<BranchCache, long, IdentityDbContext>, IBranchCacheRepository
    {
        private readonly IdentityDbContext _context;

        public BranchCacheRepository(IdentityDbContext context, IUnitOfWork<IdentityDbContext> unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
        }

        public async Task<BranchCache?> GetByCodeAsync(string code)
        {
            return await _context.Set<BranchCache>()
                .FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<List<BranchCache>> GetActiveBranchesAsync()
        {
            return await _context.Set<BranchCache>()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            var count = await _context.Set<BranchCache>()
                .Where(x => x.Code == code)
                .CountAsync();
            return count > 0;
        }

        public async Task<BranchCache?> GetByIdAsync(long id)
        {
            return await _context.Set<BranchCache>()
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
