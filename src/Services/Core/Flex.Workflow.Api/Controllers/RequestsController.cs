using Flex.Shared.SeedWork;
using Flex.Workflow.Api.Models.Requests;
using Flex.Workflow.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Flex.Workflow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _service;

        public RequestsController(IRequestService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApprovalRequestDto dto, CancellationToken ct)
        {
            var id = await _service.CreateAsync(dto, ct);
            return Ok(new { RequestId = id });
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending([FromQuery] GetPendingRequestsPagingRequest request, CancellationToken ct)
        {
            var result = await _service.GetPendingPagedAsync(request, ct);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetDetail([FromRoute] long id, CancellationToken ct)
        {
            var result = await _service.GetDetailAsync(id, ct);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{id:long}/approve")]
        public async Task<IActionResult> Approve([FromRoute] long id, [FromBody] ApproveRequestDto dto, CancellationToken ct)
        {
            await _service.ApproveAsync(id, dto, ct);
            return Ok(Result.Success());
        }

        [Authorize]
        [HttpPost("{id:long}/reject")]
        public async Task<IActionResult> Reject([FromRoute] long id, [FromBody] RejectRequestDto dto, CancellationToken ct)
        {
            await _service.RejectAsync(id, dto, ct);
            return Ok(Result.Success());
        }
    }
}
