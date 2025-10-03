using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.Workflow.Persistence;
using Flex.Infrastructure.Workflow.Persistence.Entities;
using Flex.Workflow.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Repositories
{
    public class WorkflowRequestRepository : RepositoryBase<WorkflowRequest, long, WorkflowDbContext>, IWorkflowRequestRepository
    {
        private readonly WorkflowDbContext _dbContext;
        public WorkflowRequestRepository(WorkflowDbContext dbContext, IUnitOfWork<WorkflowDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsByCorrelationIdAsync(string correlationId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(correlationId)) return false;

            return await _dbContext.Requests.AsNoTracking()
                .AnyAsync(r => r.CorrelationId == correlationId, ct);
        }

        public async Task<WorkflowRequest?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            return await _dbContext.Requests.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }
    }
}

