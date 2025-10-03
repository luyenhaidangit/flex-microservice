using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = true, CancellationToken ct = default)
        {
            var list = await _service.GetAllAsync(onlyActive, ct);
            return Ok(list);
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
    }
}

