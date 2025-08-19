using Microsoft.EntityFrameworkCore;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.System.Api.Entities;
using Flex.System.Api.Models;
using Flex.System.Api.Repositories.Interfaces;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow.Constants;
using Flex.System.Api.Persistence;
using Flex.Infrastructure.EF;
using Flex.System.Api.Models.Branch;

namespace Flex.System.Api.Repositories
{
    public class BranchRequestRepository : RepositoryBase<BranchRequest, long, SystemDbContext>, IBranchRequestRepository
    {
        private readonly SystemDbContext _context;
        public BranchRequestRepository(SystemDbContext dbContext, IUnitOfWork<SystemDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _context = dbContext;
        }

        public async Task<PagedResult<BranchPendingPagingDto>> GetPendingPagedAsync(GetBranchPagingRequest request)
        {
            // ===== Process request parameters =====
            var keyword = request?.Keyword?.Trim().ToLower();
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            // ===== Build query =====
            var query = _context.BranchRequests
                .Where(x => x.Status == RequestStatusConstant.Unauthorised);

            // ===== Execute query =====
            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.RequestedDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BranchPendingPagingDto
                {
                    Id = x.Id,
                    //EntityCode = x.EntityCode,
                    Action = x.Action,
                    Status = x.Status,
                    CreatedBy = x.MakerId,
                    CreatedDate = x.RequestedDate
                })
                .ToListAsync();

            // ===== Return result =====
            return PagedResult<BranchPendingPagingDto>.Create(pageIndex, pageSize, total, items);
        }

        public async Task<BranchRequest?> GetPendingByIdAsync(long requestId)
        {
            return await _context.BranchRequests
                .FirstOrDefaultAsync(x => x.Id == requestId && x.Status == RequestStatusConstant.Unauthorised);
        }
    }
}
