using Flex.Infrastructure.Workflow.Abstractions.DTOs;

namespace Flex.Workflow.Api.Services.Interfaces
{
    public interface IWorkflowRequestService
    {
        Task<long> SubmitAsync(CreateWorkflowRequest request, CancellationToken ct = default);
        Task<bool> ApproveAsync(long requestId, string? comment, CancellationToken ct = default);
        Task<bool> RejectAsync(long requestId, string? comment, CancellationToken ct = default);
        Task<bool> CancelAsync(long requestId, string? comment, CancellationToken ct = default);
        Task<WorkflowRequestDto?> GetAsync(long requestId, CancellationToken ct = default);
    }
}

