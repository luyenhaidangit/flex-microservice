using Flex.Contracts.Domains.Interfaces;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;

namespace Flex.Workflow.Api.Repositories.Interfaces
{
    public interface IWorkflowActionRepository : IRepositoryBase<WorkflowAction, long, WorkflowDbContext>
    {
        Task<List<WorkflowAction>> GetByRequestAsync(long requestId, CancellationToken ct = default);
    }
}
