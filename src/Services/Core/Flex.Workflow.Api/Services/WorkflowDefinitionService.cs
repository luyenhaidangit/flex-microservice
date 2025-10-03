using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Repositories.Interfaces;
using Flex.Workflow.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Services
{
    public class WorkflowDefinitionService : IWorkflowDefinitionService
    {
        private readonly IWorkflowDefinitionRepository _repo;

        public WorkflowDefinitionService(IWorkflowDefinitionRepository repo)
        {
            _repo = repo;
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
                await _repo.CreateAsync(input);
                return input;
            }
        }
    }
}

