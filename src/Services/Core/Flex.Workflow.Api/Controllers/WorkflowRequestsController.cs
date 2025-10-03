using Flex.Infrastructure.Workflow.Abstractions.DTOs;
using Flex.Shared.SeedWork;
using Flex.Workflow.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flex.Workflow.Api.Controllers
{
    [Route("api/workflow/requests")]
    [ApiController]
    [Authorize]
    public class WorkflowRequestsController : ControllerBase
    {
        private readonly IWorkflowRequestService _service;
        public WorkflowRequestsController(IWorkflowRequestService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] CreateWorkflowRequest request, CancellationToken ct)
        {
            var id = await _service.SubmitAsync(request, ct);
            return Ok(Result.Success(id));
        }

        [HttpPost("{id:long}/approve")]
        public async Task<IActionResult> Approve(long id, [FromBody] ApproveRejectDto? dto, CancellationToken ct)
        {
            var ok = await _service.ApproveAsync(id, dto?.Comment, ct);
            return Ok(Result.Success(ok));
        }

        [HttpPost("{id:long}/reject")]
        public async Task<IActionResult> Reject(long id, [FromBody] ApproveRejectDto? dto, CancellationToken ct)
        {
            var ok = await _service.RejectAsync(id, dto?.Comment, ct);
            return Ok(Result.Success(ok));
        }

        [HttpPost("{id:long}/cancel")]
        public async Task<IActionResult> Cancel(long id, [FromBody] ApproveRejectDto? dto, CancellationToken ct)
        {
            var ok = await _service.CancelAsync(id, dto?.Comment, ct);
            return Ok(Result.Success(ok));
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id, CancellationToken ct)
        {
            var dto = await _service.GetAsync(id, ct);
            return Ok(Result.Success(dto));
        }
    }

    public class ApproveRejectDto
    {
        public string? Comment { get; set; }
    }
}

