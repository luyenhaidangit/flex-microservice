using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Repositories.Interfaces;
using Flex.Workflow.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Services
{
    public class WorkflowDefinitionService : IWorkflowDefinitionService
    {
        private readonly IWorkflowDefinitionRepository _repo;
        private readonly IWorkflowDefinitionPublishRequestRepository _pubRepo;

        public WorkflowDefinitionService(IWorkflowDefinitionRepository repo, IWorkflowDefinitionPublishRequestRepository pubRepo)
        {
            _repo = repo;
            _pubRepo = pubRepo;
        }

        public async Task<List<WorkflowDefinition>> GetAllAsync(bool onlyActive = true, CancellationToken ct = default)
        {
            var query = _repo.FindAll().OrderByDescending(x => x.IsActive).ThenBy(x => x.Code).ThenByDescending(x => x.Version);
            if (onlyActive) query = query.Where(x => x.IsActive).OrderBy(x => x.Code).ThenByDescending(x => x.Version);
            return await query.AsNoTracking().ToListAsync(ct);
        }

        public Task<WorkflowDefinition?> GetActiveByCodeAsync(string code, CancellationToken ct = default)
        {
            return _repo.GetActiveByCodeAsync(code, ct);
        }

        public async Task<WorkflowDefinition> UpsertAsync(WorkflowDefinition input, string updatedBy, CancellationToken ct = default)
        {
            input.UpdatedAt = DateTime.UtcNow;
            input.UpdatedBy = updatedBy;

            // If Id provided -> update; otherwise insert new
            if (input.Id > 0)
            {
                await _repo.UpdateAsync(input);
                return input;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(input.State)) input.State = "D"; // Draft
                await _repo.CreateAsync(input);
                return input;
            }
        }

        public async Task<List<WorkflowDefinition>> GetVersionsAsync(string code, CancellationToken ct = default)
        {
            return await _repo.FindAll()
                .Where(x => x.Code == code)
                .OrderByDescending(x => x.Version)
                .ThenByDescending(x => x.UpdatedAt)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<long> RequestPublishAsync(string code, int version, string makerId, string? comment, CancellationToken ct = default)
        {
            var def = await _repo.FindAll().FirstOrDefaultAsync(x => x.Code == code && x.Version == version, ct)
                ?? throw new Exception($"Definition {code} v{version} not found");

            if (def.State.Equals("A", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Definition already active");

            var pending = await _pubRepo.GetPendingAsync(code, version, ct);
            if (pending != null) return pending.Id;

            var req = new WorkflowDefinitionPublishRequest
            {
                Code = code,
                Version = version,
                MakerId = makerId,
                Status = "UNA",
                RequestComment = comment
            };
            await _pubRepo.CreateAsync(req);
            return req.Id;
        }

        public async Task ApprovePublishAsync(long requestId, string approverId, string? comment, CancellationToken ct = default)
        {
            var req = await _pubRepo.GetByIdAsync(requestId) ?? throw new Exception("Publish request not found");
            if (!string.Equals(req.Status, "UNA", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Request is not pending");

            // Update states: set requested version Active, others with same code Deprecated
            var defs = await _repo.FindAll().Where(x => x.Code == req.Code).ToListAsync(ct);
            foreach (var d in defs)
            {
                if (d.Version == req.Version)
                {
                    d.State = "A"; // Active
                    d.IsActive = true;
                }
                else if (d.State.Equals("A", StringComparison.OrdinalIgnoreCase))
                {
                    d.State = "X"; // Deprecated
                    d.IsActive = false;
                }
            }
            await _repo.UpdateListAsync(defs);

            req.Status = "AUT";
            req.CheckerId = approverId;
            req.ApprovedAt = DateTime.UtcNow;
            req.RequestComment = comment ?? req.RequestComment;
            await _pubRepo.UpdateAsync(req);
        }

        public async Task RejectPublishAsync(long requestId, string approverId, string? comment, CancellationToken ct = default)
        {
            var req = await _pubRepo.GetByIdAsync(requestId) ?? throw new Exception("Publish request not found");
            if (!string.Equals(req.Status, "UNA", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Request is not pending");

            req.Status = "REJ";
            req.CheckerId = approverId;
            req.ApprovedAt = DateTime.UtcNow;
            req.RequestComment = comment ?? req.RequestComment;
            await _pubRepo.UpdateAsync(req);
        }
    }
}
