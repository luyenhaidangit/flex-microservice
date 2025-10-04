using Flex.Contracts.Domains.Interfaces;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;

namespace Flex.Workflow.Api.Repositories.Interfaces
{
    public interface IWorkflowDefinitionPublishRequestRepository : IRepositoryBase<WorkflowDefinitionPublishRequest, long, WorkflowDbContext>
    {
        Task<WorkflowDefinitionPublishRequest?> GetPendingAsync(string code, int version, CancellationToken ct = default);
    }
}

