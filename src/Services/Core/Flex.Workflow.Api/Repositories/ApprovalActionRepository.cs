using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;
using Flex.Workflow.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Repositories
{
    public class ApprovalActionRepository : RepositoryBase<ApprovalAction, long, WorkflowDbContext>, IApprovalActionRepository
    {
        private readonly WorkflowDbContext _context;
        public ApprovalActionRepository(WorkflowDbContext dbContext, IUnitOfWork<WorkflowDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _context = dbContext;
        }

        public Task<List<ApprovalAction>> GetByRequestAsync(long requestId, CancellationToken ct = default)
        {
            return _context.ApprovalActions.AsNoTracking()
                .Where(x => x.RequestId == requestId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(ct);
        }
    }
}

