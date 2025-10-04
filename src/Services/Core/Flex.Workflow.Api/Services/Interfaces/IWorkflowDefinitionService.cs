using Flex.Shared.SeedWork;
using Flex.Workflow.Api.Entities;

namespace Flex.Workflow.Api.Services.Interfaces
{
    public interface IWorkflowDefinitionService
    {
        Task<WorkflowDefinition> UpsertAsync(WorkflowDefinition input, string updatedBy, CancellationToken ct = default);
        Task<List<WorkflowDefinition>> GetAllAsync(bool onlyActive = true, CancellationToken ct = default);
        Task<WorkflowDefinition?> GetActiveByCodeAsync(string code, CancellationToken ct = default);
        Task<List<WorkflowDefinition>> GetVersionsAsync(string code, CancellationToken ct = default);
        Task<long> RequestPublishAsync(string code, int version, string makerId, string? comment, CancellationToken ct = default);
        Task ApprovePublishAsync(long requestId, string approverId, string? comment, CancellationToken ct = default);
        Task RejectPublishAsync(long requestId, string approverId, string? comment, CancellationToken ct = default);
    }
}
