using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Workflow.Persistence;
using Flex.Infrastructure.Workflow.Persistence.Entities;

namespace Flex.Workflow.Api.Repositories.Interfaces
{
    public interface IWorkflowStepRepository : IRepositoryBase<WorkflowStep, long, WorkflowDbContext>
    {
        Task<IReadOnlyList<WorkflowStep>> GetByRequestAsync(long requestId, CancellationToken ct = default);
    }
}

