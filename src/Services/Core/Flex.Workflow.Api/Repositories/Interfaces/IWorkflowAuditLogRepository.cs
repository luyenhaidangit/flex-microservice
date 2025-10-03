using Flex.Contracts.Domains.Interfaces;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;

namespace Flex.Workflow.Api.Repositories.Interfaces
{
    public interface IWorkflowAuditLogRepository : IRepositoryBase<WorkflowAuditLog, long, WorkflowDbContext>
    {
        Task<WorkflowAuditLog?> GetLastHashAsync(long requestId, CancellationToken ct = default);
        Task<List<WorkflowAuditLog>> GetByRequestAsync(long requestId, CancellationToken ct = default);
    }
}

