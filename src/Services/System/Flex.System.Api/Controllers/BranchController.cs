using Microsoft.AspNetCore.Mvc;
using Flex.System.Api.Models;
using Flex.System.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Flex.System.Api.Models.Branch;

namespace Flex.System.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        /// <summary>
        /// Get all approved branches with pagination.
        /// </summary>
        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedBranches([FromQuery] GetBranchPagingRequest request)
        {
            var result = await _branchService.GetApprovedBranchesPagedAsync(request);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get approved branch by code.
        /// </summary>
        [HttpGet("approved/{code}")]
        public async Task<IActionResult> GetApprovedBranchByCode(string code)
        {
            var result = await _branchService.GetApprovedBranchByCodeAsync(code);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get approved branch history by code.
        /// </summary>
        [HttpGet("approved/{code}/history")]
        public async Task<IActionResult> GetApprovedBranchChangeHistory(string code)
        {
            var result = await _branchService.GetApprovedBranchChangeHistoryAsync(code);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get all approved and active branches for filter dropdown.
        /// </summary>
        [HttpGet("filter")]
        public async Task<IActionResult> GetBranchesForFilter()
        {
            var result = await _branchService.GetBranchesForFilterAsync();
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Create branch request.
        /// </summary>
        [HttpPost("requests/create")]
        public async Task<IActionResult> CreateBranchRequest([FromBody] CreateBranchRequestDto request)
        {
            var requestId = await _branchService.CreateBranchRequestAsync(request);
            return Ok(Result.Success(requestId));
        }

        /// <summary>
        /// Create update branch request.
        /// </summary>
        [HttpPost("approved/{code}/update")]
        public async Task<IActionResult> CreateUpdateBranchRequest(string code, [FromBody] UpdateBranchRequestDto request)
        {
            var requestId = await _branchService.CreateUpdateBranchRequestAsync(code, request);
            return Ok(Result.Success(requestId));
        }

        /// <summary>
        /// Create delete branch request.
        /// </summary>
        [HttpPost("approved/{code}/delete")]
        public async Task<IActionResult> CreateDeleteBranchRequest(string code, [FromBody] DeleteBranchRequestDto request)
        {
            var requestId = await _branchService.CreateDeleteBranchRequestAsync(code, request);
            return Ok(Result.Success(requestId));
        }

        /// <summary>
        /// Get pending branches with pagination.
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingBranches([FromQuery] GetBranchPagingRequest request)
        {
            var result = await _branchService.GetPendingBranchesPagedAsync(request);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get pending branch request by ID.
        /// </summary>
        [HttpGet("pending/{requestId}")]
        public async Task<IActionResult> GetPendingBranchById(long requestId)
        {
            var result = await _branchService.GetPendingBranchByIdAsync(requestId);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Approve pending branch request by ID.
        /// </summary>
        [HttpPost("pending/{requestId}/approve")]
        public async Task<IActionResult> ApprovePendingBranchRequest(long requestId, [FromBody] ApproveBranchRequestDto? dto = null)
        {
            var result = await _branchService.ApprovePendingBranchRequestAsync(requestId, dto?.Comment);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Reject pending branch request by ID.
        /// </summary>
        [HttpPost("pending/{requestId}/reject")]
        public async Task<IActionResult> RejectPendingBranchRequest(long requestId, [FromBody] RejectBranchRequestDto? dto = null)
        {
            var result = await _branchService.RejectPendingBranchRequestAsync(requestId, dto?.Reason);
            return Ok(Result.Success(result));
        }
    }
}
