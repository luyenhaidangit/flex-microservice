using Flex.Contracts.Domains.Interfaces;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;

namespace Flex.Workflow.Api.Repositories.Interfaces
{
    public interface IWorkflowDefinitionRepository : IRepositoryBase<WorkflowDefinition, long, WorkflowDbContext>
    {
        Task<WorkflowDefinition?> GetActiveByCodeAsync(string code, CancellationToken ct = default);
    }
}

