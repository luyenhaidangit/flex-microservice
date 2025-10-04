using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;
using Flex.Workflow.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Repositories
{
    public class WorkflowDefinitionPublishRequestRepository : RepositoryBase<WorkflowDefinitionPublishRequest, long, WorkflowDbContext>, IWorkflowDefinitionPublishRequestRepository
    {
        private readonly WorkflowDbContext _context;
        public WorkflowDefinitionPublishRequestRepository(WorkflowDbContext dbContext, IUnitOfWork<WorkflowDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _context = dbContext;
        }

        public Task<WorkflowDefinitionPublishRequest?> GetPendingAsync(string code, int version, CancellationToken ct = default)
        {
            return _context.Set<WorkflowDefinitionPublishRequest>()
                .FirstOrDefaultAsync(x => x.Code == code && x.Version == version && x.Status == "UNA", ct);
        }
    }
}

