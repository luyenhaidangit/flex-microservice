using Flex.Infrastructure.Workflow.Abstractions.DTOs;
using Flex.Infrastructure.Workflow.Abstractions.Enums;
using Flex.Infrastructure.Workflow.Abstractions.Interfaces;
using Flex.Infrastructure.Workflow.Abstractions.Interfaces.Stores;
using Flex.Infrastructure.Workflow.Core.Models;
using Flex.Infrastructure.Workflow.Persistence.Entities;

namespace Flex.Infrastructure.Workflow.Core;

public class WorkflowService : IWorkflowService
{
    private readonly IRequestStore _requestStore;
    private readonly IStepStore _stepStore;

    public WorkflowService(IRequestStore requestStore, IStepStore stepStore)
    {
        _requestStore = requestStore;
        _stepStore = stepStore;
    }

    public async Task<long> SubmitAsync(CreateWorkflowRequest request, CancellationToken ct = default)
    {
        var entity = new WorkflowRequest
        {
            WorkflowType = request.WorkflowType,
            EntityId = request.EntityId,
            Status = WorkflowStatus.Pending,
            RequestedBy = "system", // integrate ICurrentUserContext later
            RequestedAt = DateTime.UtcNow,
            PayloadJson = request.PayloadJson,
            Comment = request.Comment,
            CorrelationId = request.CorrelationId
        };

        var id = await _requestStore.CreateAsync(entity, ct);
        return id;
    }

    public async Task ApproveAsync(long requestId, string? comment, CancellationToken ct = default)
    {
        var req = await _requestStore.GetAsync(requestId, ct) ?? throw new InvalidOperationException("Request not found");
        req.Status = WorkflowStatus.Approved;
        await _requestStore.UpdateAsync(req, ct);
    }

    public async Task RejectAsync(long requestId, string? comment, CancellationToken ct = default)
    {
        var req = await _requestStore.GetAsync(requestId, ct) ?? throw new InvalidOperationException("Request not found");
        req.Status = WorkflowStatus.Rejected;
        await _requestStore.UpdateAsync(req, ct);
    }

    public async Task CancelAsync(long requestId, string? comment, CancellationToken ct = default)
    {
        var req = await _requestStore.GetAsync(requestId, ct) ?? throw new InvalidOperationException("Request not found");
        req.Status = WorkflowStatus.Cancelled;
        await _requestStore.UpdateAsync(req, ct);
    }

    public async Task<WorkflowRequestDto?> GetAsync(long requestId, CancellationToken ct = default)
    {
        var req = await _requestStore.GetAsync(requestId, ct);
        if (req is null) return null;
        return new WorkflowRequestDto
        {
            Id = req.Id,
            WorkflowType = req.WorkflowType,
            EntityId = req.EntityId,
            Status = req.Status,
            RequestedBy = req.RequestedBy,
            RequestedAt = req.RequestedAt,
            Comment = req.Comment
        };
    }
}

