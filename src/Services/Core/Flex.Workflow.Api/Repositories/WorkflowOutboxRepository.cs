using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;
using Flex.Workflow.Api.Repositories.Interfaces;

namespace Flex.Workflow.Api.Repositories
{
    public class WorkflowOutboxRepository : RepositoryBase<WorkflowOutboxEvent, long, WorkflowDbContext>, IWorkflowOutboxRepository
    {
        public WorkflowOutboxRepository(WorkflowDbContext dbContext, IUnitOfWork<WorkflowDbContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }
    }
}
