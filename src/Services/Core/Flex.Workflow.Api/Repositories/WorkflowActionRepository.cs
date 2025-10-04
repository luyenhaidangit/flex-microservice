using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;
using Flex.Workflow.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Repositories
{
    public class WorkflowActionRepository : RepositoryBase<WorkflowAction, long, WorkflowDbContext>, IWorkflowActionRepository
    {
        private readonly WorkflowDbContext _context;
        public WorkflowActionRepository(WorkflowDbContext dbContext, IUnitOfWork<WorkflowDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _context = dbContext;
        }

        public Task<List<WorkflowAction>> GetByRequestAsync(long requestId, CancellationToken ct = default)
        {
            return _context.WorkflowActions.AsNoTracking()
                .Where(x => x.RequestId == requestId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(ct);
        }
    }
}
