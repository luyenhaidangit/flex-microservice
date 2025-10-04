using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;
using Flex.Workflow.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Repositories
{
    public class WorkflowDefinitionRepository : RepositoryBase<WorkflowDefinition, long, WorkflowDbContext>, IWorkflowDefinitionRepository
    {
        private readonly WorkflowDbContext _context;
        public WorkflowDefinitionRepository(WorkflowDbContext dbContext, IUnitOfWork<WorkflowDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _context = dbContext;
        }

        public Task<WorkflowDefinition?> GetActiveByCodeAsync(string code, CancellationToken ct = default)
        {
            return _context.WorkflowDefinitions
                .AsNoTracking()
                .Where(x => x.Code == code && x.IsActive)
                .OrderByDescending(x => x.Version)
                .FirstOrDefaultAsync(ct);
        }
    }
}
