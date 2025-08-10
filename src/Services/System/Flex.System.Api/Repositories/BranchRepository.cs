using Microsoft.EntityFrameworkCore;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.System.Api.Entities;
using Flex.System.Api.Models;
using Flex.System.Api.Repositories.Interfaces;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow.Constants;
using Flex.Shared.Constants.Common;
using Flex.System.Api.Persistence;
using Flex.Infrastructure.EF;

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

        public async Task<PagedResult<BranchListItemDto>> GetPagedAsync(GetBranchPagingRequest request)
        {
            // ===== Process request parameters =====
            var keyword = request?.Keyword?.Trim().ToLower();
            var status = request?.IsActive?.Trim().ToUpper() == "Y" ? true : false;
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            // ===== Build query =====
            var query = _context.Branches
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    x => EF.Functions.Like(x.Code.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like(x.Name.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like((x.Description ?? "").ToLower(), $"%{keyword}%"))
                .WhereIf(!string.IsNullOrEmpty(request.IsActive), x => x.IsActive == status);

            // ===== Execute query =====
            var total = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BranchListItemDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description ?? string.Empty,
                    Status = x.Status,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            // ===== Return result =====
            return PagedResult<BranchListItemDto>.Create(pageIndex, pageSize, total, items);
        }

        public async Task<PagedResult<BranchListItemDto>> GetApprovedPagedAsync(GetBranchPagingRequest request)
        {
            // ===== Process request parameters =====
            var keyword = request?.Keyword?.Trim().ToLower();
            var status = request?.IsActive?.Trim().ToUpper() == "Y" ? true : false;
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            // ===== Build query =====
            var query = _context.Branches
                .Where(x => x.Status == StatusConstant.Approved)
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    x => EF.Functions.Like(x.Code.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like(x.Name.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like((x.Description ?? "").ToLower(), $"%{keyword}%"))
                .WhereIf(!string.IsNullOrEmpty(request.IsActive), x => x.IsActive == status);

            // ===== Execute query =====
            var total = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BranchListItemDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description ?? string.Empty,
                    Status = x.Status,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            // ===== Return result =====
            return PagedResult<BranchListItemDto>.Create(pageIndex, pageSize, total, items);
        }
    }
}
