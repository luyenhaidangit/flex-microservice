using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.Workflow.Persistence;
using Flex.Infrastructure.Workflow.Persistence.Entities;
using Flex.Workflow.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Repositories
{
    public class WorkflowStepRepository : RepositoryBase<WorkflowStep, long, WorkflowDbContext>, IWorkflowStepRepository
    {
        private readonly WorkflowDbContext _dbContext;
        public WorkflowStepRepository(WorkflowDbContext dbContext, IUnitOfWork<WorkflowDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<WorkflowStep>> GetByRequestAsync(long requestId, CancellationToken ct = default)
        {
            return await _dbContext.Steps.AsNoTracking()
                .Where(s => s.RequestId == requestId)
                .OrderBy(s => s.Level).ThenBy(s => s.Order)
                .ToListAsync(ct);
        }
    }
}

