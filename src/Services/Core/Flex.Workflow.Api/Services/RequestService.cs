using AutoMapper;
using Flex.Infrastructure.Workflow.Constants;
using Flex.Shared.SeedWork;
using Flex.Shared.Extensions;
using Flex.Infrastructure.EF;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Models.Requests;
using Flex.Workflow.Api.Repositories.Interfaces;
using Flex.Workflow.Api.Services.Interfaces;
using Flex.Workflow.Api.Services.Policy;
using Flex.Workflow.Api.Services.Steps;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Flex.Workflow.Api.Services
{
    public class RequestService : IRequestService
    {
        private readonly IApprovalRequestRepository _requestRepo;
        private readonly IApprovalActionRepository _actionRepo;
        private readonly IWorkflowAuditLogRepository _auditRepo;
        private readonly IOutboxRepository _outboxRepo;
        private readonly IWorkflowDefinitionRepository _definitionRepo;
        private readonly IMapper _mapper;
        private readonly IPolicyEvaluator _policy;
        private readonly IStepResolver _steps;

        public RequestService(
            IApprovalRequestRepository requestRepo,
            IApprovalActionRepository actionRepo,
            IWorkflowAuditLogRepository auditRepo,
            IOutboxRepository outboxRepo,
            IWorkflowDefinitionRepository definitionRepo,
            IMapper mapper,
            IPolicyEvaluator policy,
            IStepResolver steps)
        {
            _requestRepo = requestRepo;
            _actionRepo = actionRepo;
            _auditRepo = auditRepo;
            _outboxRepo = outboxRepo;
            _definitionRepo = definitionRepo;
            _mapper = mapper;
            _policy = policy;
            _steps = steps;
        }

        public async Task<long> CreateAsync(CreateApprovalRequestDto dto, CancellationToken ct = default)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(dto.Domain)) throw new ArgumentException("Domain is required");
            if (string.IsNullOrWhiteSpace(dto.WorkflowCode)) throw new ArgumentException("WorkflowCode is required");
            if (string.IsNullOrWhiteSpace(dto.Action)) throw new ArgumentException("Action is required");
            if (string.IsNullOrWhiteSpace(dto.MakerId)) throw new ArgumentException("MakerId is required");

            var definition = await _definitionRepo.GetActiveByCodeAsync(dto.WorkflowCode, ct)
                ?? throw new InvalidOperationException($"Workflow definition '{dto.WorkflowCode}' not found or inactive.");

            var request = new ApprovalRequest
            {
                Domain = dto.Domain.Trim().ToUpperInvariant(),
                WorkflowCode = dto.WorkflowCode.Trim(),
                Action = dto.Action.Trim().ToUpperInvariant(),
                Status = RequestStatus.Unauthorised,
                EntityId = 0,
                MakerId = dto.MakerId,
                RequestedDate = DateTime.UtcNow,
                Comments = dto.Comments,
                RequestedData = dto.Payload == null ? string.Empty : JsonSerializer.Serialize(dto.Payload),
                BusinessId = string.IsNullOrWhiteSpace(dto.BusinessId) ? null : dto.BusinessId,
                CorrelationId = string.IsNullOrWhiteSpace(dto.CorrelationId) ? null : dto.CorrelationId
            };

            await _requestRepo.CreateAsync(request);

            // Policy evaluation
            JsonDocument? payloadDoc = null;
            if (!string.IsNullOrWhiteSpace(request.RequestedData))
            {
                try { payloadDoc = JsonDocument.Parse(request.RequestedData); } catch { }
            }
            var decision = _policy.Evaluate(definition.Policy, payloadDoc);

            // Steps parse
            var stepDoc = _steps.Parse(definition.Steps);
            var totalStages = _steps.GetCurrentStage(Array.Empty<ApprovalAction>(), stepDoc).totalStages;

            if (decision.Result.Equals("auto", StringComparison.OrdinalIgnoreCase) && totalStages == 0)
            {
                // Auto-approve when no manual steps defined
                request.Status = RequestStatus.Authorised;
                request.CheckerId = "policy:auto";
                request.ApproveDate = DateTime.UtcNow;
                await _requestRepo.UpdateAsync(request);

                await WriteAuditAsync(request.Id, "request.auto_approved", request.CheckerId!, new { decision = decision.Result }, ct);
                await _outboxRepo.CreateAsync(new OutboxEvent
                {
                    Aggregate = "workflow.request",
                    AggregateId = request.Id.ToString(),
                    EventType = "request.auto_approved",
                    Payload = JsonSerializer.Serialize(new { request.Id, request.Domain, request.WorkflowCode, request.Action, request.Status })
                });
            }
            else
            {
                // Audit genesis
                await WriteAuditAsync(request.Id, "request.created", dto.MakerId, new { dto.Domain, dto.WorkflowCode, dto.Action, dto.BusinessId }, ct);

                // Outbox
                await _outboxRepo.CreateAsync(new OutboxEvent
                {
                    Aggregate = "workflow.request",
                    AggregateId = request.Id.ToString(),
                    EventType = "request.created",
                    Payload = JsonSerializer.Serialize(new { request.Id, request.Domain, request.WorkflowCode, request.Action, request.Status, request.MakerId, request.RequestedDate })
                });
            }

            return request.Id;
        }

        public async Task<PagedResult<PendingRequestListItemDto>> GetPendingPagedAsync(GetPendingRequestsPagingRequest request, CancellationToken ct = default)
        {
            var pageIndex = Math.Max(1, request.PageIndex ?? 1);
            var pageSize = Math.Max(1, request.PageSize ?? 10);
            var keyword = request.Keyword?.Trim().ToLowerInvariant();
            var domain = request.Domain?.Trim().ToUpperInvariant();
            var workflow = request.WorkflowCode?.Trim();
            var action = request.Action?.Trim().ToUpperInvariant();

            var query = _requestRepo.QueryPending()
                .WhereIf(!string.IsNullOrEmpty(domain), x => x.Domain == domain)
                .WhereIf(!string.IsNullOrEmpty(workflow), x => x.WorkflowCode == workflow)
                .WhereIf(!string.IsNullOrEmpty(action), x => x.Action == action)
                .WhereIf(!string.IsNullOrEmpty(keyword), x => (x.Comments ?? string.Empty).ToLower().Contains(keyword!) || (x.BusinessId ?? string.Empty).ToLower().Contains(keyword!));

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(x => x.RequestedDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => _mapper.Map<PendingRequestListItemDto>(x))
                .ToListAsync(ct);

            return PagedResult<PendingRequestListItemDto>.Create(pageIndex, pageSize, total, items);
        }

        public async Task<RequestDetailDto> GetDetailAsync(long requestId, CancellationToken ct = default)
        {
            var req = await _requestRepo.GetByIdAsync(requestId) ?? throw new KeyNotFoundException($"Request {requestId} not found");
            var actions = await _actionRepo.GetByRequestAsync(requestId, ct);
            var audits = await _auditRepo.GetByRequestAsync(requestId, ct);
            var result = _mapper.Map<RequestDetailDto>(req);
            if (!string.IsNullOrWhiteSpace(req.RequestedData))
            {
                try { result.Payload = JsonSerializer.Deserialize<object>(req.RequestedData); } catch { /* ignore */ }
            }
            result.Actions = actions.Select(_mapper.Map<RequestActionDto>).ToList();
            result.Audits = audits.Select(_mapper.Map<RequestAuditDto>).ToList();
            return result;
        }

        public async Task ApproveAsync(long requestId, ApproveRequestDto dto, CancellationToken ct = default)
        {
            var request = await _requestRepo.GetPendingByIdAsync(requestId, ct)
                ?? throw new InvalidOperationException($"Pending request {requestId} not found.");

            // In a real engine, determine the required step; for MVP, step = count(actions)+1
            var existingActions = await _actionRepo.GetByRequestAsync(requestId, ct);

            // Resolve stage & steps
            var definition = await _definitionRepo.GetActiveByCodeAsync(request.WorkflowCode, ct) ?? new Entities.WorkflowDefinition();
            var stepDoc = _steps.Parse(definition.Steps);
            var (currentStage, totalStages) = _steps.GetCurrentStage(existingActions, stepDoc);
            var nextStep = Math.Min(currentStage + 1, Math.Max(1, totalStages));

            // SoD (maker != approver)
            if (string.Equals(request.MakerId, dto.ApproverId, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Approver cannot be the same as maker (SoD)");

            var action = new ApprovalAction
            {
                RequestId = requestId,
                Step = nextStep,
                Action = "APPROVE",
                ActorId = dto.ApproverId,
                Comment = dto.Comment,
                EvidenceUrl = dto.EvidenceUrl,
                CreatedAt = DateTime.UtcNow
            };
            await _actionRepo.CreateAsync(action);

            // Check if stage completes and if this is final stage
            var newActions = await _actionRepo.GetByRequestAsync(requestId, ct);
            var stageComplete = _steps.IsStageComplete(nextStep - 1, newActions, stepDoc);
            if (stageComplete && nextStep >= totalStages)
            {
                request.Status = RequestStatus.Authorised;
                request.CheckerId = dto.ApproverId;
                request.ApproveDate = DateTime.UtcNow;
                await _requestRepo.UpdateAsync(request);
            }

            await WriteAuditAsync(requestId, "request.approved", dto.ApproverId, new { action.Step, dto.Comment }, ct);

            await _outboxRepo.CreateAsync(new OutboxEvent
            {
                Aggregate = "workflow.request",
                AggregateId = requestId.ToString(),
                EventType = request.Status == RequestStatus.Authorised ? "request.approved_final" : "request.approved_progress",
                Payload = JsonSerializer.Serialize(new { requestId, request.Domain, request.WorkflowCode, request.Action, request.Status, dto.ApproverId })
            });
        }

        public async Task RejectAsync(long requestId, RejectRequestDto dto, CancellationToken ct = default)
        {
            var request = await _requestRepo.GetPendingByIdAsync(requestId, ct)
                ?? throw new InvalidOperationException($"Pending request {requestId} not found.");

            var existingActions = await _actionRepo.GetByRequestAsync(requestId, ct);
            var nextStep = (existingActions?.Count ?? 0) + 1;

            var action = new ApprovalAction
            {
                RequestId = requestId,
                Step = nextStep,
                Action = "REJECT",
                ActorId = dto.ApproverId,
                Comment = dto.Reason,
                EvidenceUrl = dto.EvidenceUrl,
                CreatedAt = DateTime.UtcNow
            };
            await _actionRepo.CreateAsync(action);

            request.Status = RequestStatus.Rejected;
            request.CheckerId = dto.ApproverId;
            request.ApproveDate = DateTime.UtcNow;
            request.Comments = dto.Reason ?? request.Comments;
            await _requestRepo.UpdateAsync(request);

            await WriteAuditAsync(requestId, "request.rejected", dto.ApproverId, new { action.Step, dto.Reason }, ct);

            await _outboxRepo.CreateAsync(new OutboxEvent
            {
                Aggregate = "workflow.request",
                AggregateId = requestId.ToString(),
                EventType = "request.rejected",
                Payload = JsonSerializer.Serialize(new { requestId, request.Domain, request.WorkflowCode, request.Action, request.Status, dto.ApproverId, dto.Reason })
            });
        }

        private async Task WriteAuditAsync(long requestId, string @event, string actorId, object metadata, CancellationToken ct)
        {
            var last = await _auditRepo.GetLastHashAsync(requestId, ct);
            var prev = last?.CurrHash ?? "GENESIS";
            var payload = JsonSerializer.Serialize(metadata);
            var material = $"{requestId}|{@event}|{actorId}|{payload}|{DateTime.UtcNow:o}|{prev}";
            var currHash = ComputeSha256(material);
            await _auditRepo.CreateAsync(new WorkflowAuditLog
            {
                RequestId = requestId,
                Event = @event,
                ActorId = actorId,
                Metadata = payload,
                PrevHash = prev,
                CurrHash = currHash,
                CreatedAt = DateTime.UtcNow
            });
        }

        private static string ComputeSha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
