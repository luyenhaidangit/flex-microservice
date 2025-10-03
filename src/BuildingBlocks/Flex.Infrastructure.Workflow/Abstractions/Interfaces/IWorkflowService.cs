using Flex.Infrastructure.Workflow.Abstractions.DTOs;

namespace Flex.Infrastructure.Workflow.Abstractions.Interfaces;

public interface IWorkflowService
{
    Task<long> SubmitAsync(CreateWorkflowRequest request, CancellationToken ct = default);
    Task ApproveAsync(long requestId, string? comment, CancellationToken ct = default);
    Task RejectAsync(long requestId, string? comment, CancellationToken ct = default);
    Task CancelAsync(long requestId, string? comment, CancellationToken ct = default);
    Task<WorkflowRequestDto?> GetAsync(long requestId, CancellationToken ct = default);
}

