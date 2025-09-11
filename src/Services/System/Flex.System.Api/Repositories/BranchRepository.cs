using Microsoft.EntityFrameworkCore;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.System.Api.Entities;
using Flex.System.Api.Repositories.Interfaces;
using Flex.System.Api.Persistence;
using Flex.System.Api.Entities.Views;

namespace Flex.System.Api.Repositories
{
    public class BranchRepository : RepositoryBase<Branch, long, SystemDbContext>, IBranchRepository
    {
        private readonly SystemDbContext _context;
        public BranchRepository(SystemDbContext dbContext, IUnitOfWork<SystemDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _context = dbContext;
        }

        public async Task<Branch?> GetByCodeAsync(string code)
        {
            return await _context.Branches
                .FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Branches
                .CountAsync(x => x.Code == code) > 0;
        }

        public IQueryable<BrandRequestView> GetBranchCombinedQuery()
        {
            return _context.Set<BrandRequestView>().AsNoTracking();
        }
    }
}
