using Flex.Contracts.Domains.Interfaces;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;

namespace Flex.Workflow.Api.Repositories.Interfaces
{
    public interface IWorkflowOutboxRepository : IRepositoryBase<WorkflowOutboxEvent, long, WorkflowDbContext>
    {
    }
}
