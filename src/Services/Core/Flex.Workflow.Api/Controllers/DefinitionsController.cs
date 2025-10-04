using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Flex.Shared.SeedWork;

namespace Flex.Workflow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DefinitionsController : ControllerBase
    {
        private readonly IWorkflowDefinitionService _service;

        public DefinitionsController(IWorkflowDefinitionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? state = null,
            [FromQuery] bool? onlyActive = null,
            CancellationToken ct = default)
        {
            var list = await _service.GetAllAsync(onlyActive ?? false, ct);
            
            // Apply filters
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = list.Where(x => 
                    (x.Code?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.Name?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }
            
            if (!string.IsNullOrWhiteSpace(state))
            {
                list = list.Where(x => x.State == state).ToList();
            }
            
            // Apply paging
            var totalItems = list.Count;
            var pagedItems = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            
            var pagedResult = Flex.Shared.SeedWork.PagedResult<WorkflowDefinition>.Create(
                pageIndex, pageSize, totalItems, pagedItems);
            
            var result = Result.Success(pagedResult);
            
            return Ok(result);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetActiveByCode([FromRoute] string code, CancellationToken ct = default)
        {
            var def = await _service.GetActiveByCodeAsync(code, ct);
            if (def == null) return NotFound();
            return Ok(def);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] WorkflowDefinition input, CancellationToken ct = default)
        {
            // In real scenario, take user from context
            var updatedBy = User?.Identity?.Name ?? "system";
            var saved = await _service.UpsertAsync(input, updatedBy, ct);
            return Ok(saved);
        }

        [HttpGet("{code}/versions")]
        public async Task<IActionResult> GetVersions([FromRoute] string code, CancellationToken ct = default)
        {
            var list = await _service.GetVersionsAsync(code, ct);
            return Ok(list);
        }

        public class PublishRequestDto { public string? Comment { get; set; } }
        [HttpPost("{code}/publish")] // creates a maker request for the latest version of this code (or include version via query/body as needed)
        public async Task<IActionResult> CreatePublish([FromRoute] string code, [FromQuery] int version, [FromBody] PublishRequestDto dto, CancellationToken ct)
        {
            var maker = User?.Identity?.Name ?? "system";
            var id = await _service.RequestPublishAsync(code, version, maker, dto?.Comment, ct);
            return Ok(new { RequestId = id });
        }

        public class ApproveRejectDto { public string? Comment { get; set; } }
        [HttpPost("{code}/publish/{requestId:long}/approve")]
        public async Task<IActionResult> ApprovePublish([FromRoute] string code, [FromRoute] long requestId, [FromBody] ApproveRejectDto dto, CancellationToken ct)
        {
            var approver = User?.Identity?.Name ?? "system";
            await _service.ApprovePublishAsync(requestId, approver, dto?.Comment, ct);
            return Ok();
        }

        [HttpPost("{code}/publish/{requestId:long}/reject")]
        public async Task<IActionResult> RejectPublish([FromRoute] string code, [FromRoute] long requestId, [FromBody] ApproveRejectDto dto, CancellationToken ct)
        {
            var approver = User?.Identity?.Name ?? "system";
            await _service.RejectPublishAsync(requestId, approver, dto?.Comment, ct);
            return Ok();
        }

        public class SimulateRequestDto { public object? Payload { get; set; } }
        [HttpPost("{code}/simulate")]
        public async Task<IActionResult> Simulate([FromRoute] string code, [FromQuery] int? version, [FromBody] SimulateRequestDto body, [FromServices] Services.Policy.IPolicyEvaluator policy, [FromServices] Services.Steps.IStepResolver steps, CancellationToken ct)
        {
            WorkflowDefinition? def;
            if (version.HasValue)
                def = (await _service.GetVersionsAsync(code, ct)).FirstOrDefault(x => x.Version == version.Value);
            else
                def = await _service.GetActiveByCodeAsync(code, ct);

            if (def == null) return NotFound();

            JsonDocument? payloadDoc = null;
            if (body?.Payload != null)
            {
                try { payloadDoc = JsonDocument.Parse(JsonSerializer.Serialize(body.Payload)); } catch { }
            }

            var decision = policy.Evaluate(def.Policy, payloadDoc);
            var stepsDoc = steps.Parse(def.Steps);
            var stages = steps.GetCurrentStage(Enumerable.Empty<Entities.WorkflowAction>(), stepsDoc);

            return Ok(new
            {
                Code = def.Code,
                Version = def.Version,
                Decision = decision.Result,
                Stages = new { Current = stages.currentStage, Total = stages.totalStages }
            });
        }
    }
}
