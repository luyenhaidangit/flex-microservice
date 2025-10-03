using Flex.Infrastructure.Workflow.Abstractions.DTOs;
using Flex.Infrastructure.Workflow.Constants;
using Flex.Infrastructure.Workflow.Persistence.Entities;
using Flex.Workflow.Api.Repositories.Interfaces;
using Flex.Workflow.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Services
{
    public class WorkflowRequestService : IWorkflowRequestService
    {
        private readonly IWorkflowRequestRepository _requestRepo;
        private readonly IWorkflowStepRepository _stepRepo;
        private readonly ICurrentUserService _currentUser;

        public WorkflowRequestService(
            IWorkflowRequestRepository requestRepo,
            IWorkflowStepRepository stepRepo,
            ICurrentUserService currentUser)
        {
            _requestRepo = requestRepo;
            _stepRepo = stepRepo;
            _currentUser = currentUser;
        }

        public async Task<long> SubmitAsync(CreateWorkflowRequest request, CancellationToken ct = default)
        {
            if (!string.IsNullOrWhiteSpace(request.CorrelationId))
            {
                var exists = await _requestRepo.ExistsByCorrelationIdAsync(request.CorrelationId, ct);
                if (exists) return 0; // idempotent noop
            }

            var username = _currentUser.GetCurrentUsername() ?? "system";
            var entity = new WorkflowRequest
            {
                WorkflowType = request.WorkflowType,
                EntityId = request.EntityId,
                Status = RequestStatus.Unauthorised,
                RequestedBy = username,
                RequestedAt = DateTime.UtcNow,
                PayloadJson = request.PayloadJson,
                Comment = request.Comment,
                CorrelationId = request.CorrelationId
            };

            var id = await _requestRepo.CreateAsync(entity);
            return id;
        }

        public async Task<bool> ApproveAsync(long requestId, string? comment, CancellationToken ct = default)
        {
            var entity = await _requestRepo.FindByCondition(r => r.Id == requestId).FirstOrDefaultAsync(ct);
            if (entity == null) return false;
            entity.Status = RequestStatus.Authorised;
            entity.Comment = comment ?? entity.Comment;
            await _requestRepo.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> RejectAsync(long requestId, string? comment, CancellationToken ct = default)
        {
            var entity = await _requestRepo.FindByCondition(r => r.Id == requestId).FirstOrDefaultAsync(ct);
            if (entity == null) return false;
            entity.Status = RequestStatus.Rejected;
            entity.Comment = comment ?? entity.Comment;
            await _requestRepo.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> CancelAsync(long requestId, string? comment, CancellationToken ct = default)
        {
            var entity = await _requestRepo.FindByCondition(r => r.Id == requestId).FirstOrDefaultAsync(ct);
            if (entity == null) return false;
            entity.Status = RequestStatus.Cancelled;
            entity.Comment = comment ?? entity.Comment;
            await _requestRepo.UpdateAsync(entity);
            return true;
        }

        public async Task<WorkflowRequestDto?> GetAsync(long requestId, CancellationToken ct = default)
        {
            var entity = await _requestRepo.GetByIdAsync(requestId, ct);
            if (entity == null) return null;
            return new WorkflowRequestDto
            {
                Id = entity.Id,
                WorkflowType = entity.WorkflowType,
                EntityId = entity.EntityId,
                Status = entity.Status,
                RequestedBy = entity.RequestedBy,
                RequestedAt = entity.RequestedAt,
                Comment = entity.Comment
            };
        }
    }
}

