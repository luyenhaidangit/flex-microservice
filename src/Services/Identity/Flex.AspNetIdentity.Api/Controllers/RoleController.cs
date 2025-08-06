using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Flex.Shared.SeedWork;
using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Models.Requests;
using Flex.AspNetIdentity.Api.Services.Interfaces;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get all approved roles with pagination.
        /// </summary>
        [HttpGet("approved")]
        public async Task<IActionResult> GetPagingApprovedRoles([FromQuery] GetRolesPagingRequest request)
        {
            var result = await _roleService.GetApprovedRolesPagedAsync(request);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get approved role by code.
        /// </summary>
        [HttpGet("approved/{code}")]
        public async Task<IActionResult> GetApprovedRoleByCode(string code)
        {
            var result = await _roleService.GetApprovedRoleByCodeAsync(code);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get approved role history by code.
        /// </summary>
        [HttpGet("approved/{code}/history")]
        public async Task<IActionResult> GetApprovedRoleChangeHistory(string code)
        {
            var result = await _roleService.GetApprovedRoleChangeHistoryAsync(code);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Create a new role creation request.
        /// </summary>
        [HttpPost("requests/create")]
        public async Task<IActionResult> CreateRoleRequest([FromBody] CreateRoleRequestDto request)
        {
            var id = await _roleService.CreateRoleRequestAsync(request);
            return Ok(Result.Success(id));
        }

        /// <summary>
        /// Create update role request.
        /// </summary>
        [HttpPost("approved/{code}/update")]
        public async Task<IActionResult> CreateUpdateRoleRequest(string code, [FromBody] UpdateRoleRequestDto dto)
        {
            var id = await _roleService.CreateUpdateRoleRequestAsync(code, dto);
            return Ok(Result.Success(id));
        }

        /// <summary>
        /// Create delete role request.
        /// </summary>
        [HttpPost("approved/{code}/delete")]
        public async Task<IActionResult> CreateDeleteRoleRequest(string code, [FromBody] DeleteRoleRequestDto dto)
        {
            var id = await _roleService.CreateDeleteRoleRequestAsync(code, dto);
            return Ok(Result.Success(id));
        }

        /// <summary>
        /// Get all pending roles with pagination
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPagingPendingRolesAsync([FromQuery] GetRolesPagingRequest request)
        {
            var result = await _roleService.GetPendingRolesPagedAsync(request);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get role request pending detail.
        /// </summary>
        [HttpGet("pending/{requestId}")]
        public async Task<IActionResult> GetPendingRoleById(long requestId)
        {
            var result = await _roleService.GetPendingRoleByIdAsync(requestId);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Approve pending role request by ID.
        /// </summary>
        [HttpPost("pending/{requestId}/approve")]
        public async Task<IActionResult> ApprovePendingRoleRequest(long requestId, [FromBody] ApproveRoleRequestDto? dto = null)
        {
            var result = await _roleService.ApprovePendingRoleRequestAsync(requestId, dto?.Comment);
            return Ok(Result.Success(result));
        }

        //[HttpPost("requests/{requestId}/reject")]
        //public async Task<IActionResult> RejectRoleRequest(long requestId, [FromBody] RejectRoleRequestDto dto)
        //{
        //    var username = User.FindFirstValue(Flex.Security.ClaimTypes.Sub) ?? User?.Identity?.Name ?? "anonymous";
        //    await _roleService.RejectRoleRequestAsync(requestId, dto?.Reason, username);
        //    return Ok(Result.Success());
        //}

        [HttpPost("requests/{requestId}/cancel")]
        public async Task<IActionResult> CancelRoleRequest(long requestId)
        {
            var username = User.FindFirstValue(Flex.Security.ClaimTypes.Sub) ?? User?.Identity?.Name ?? "anonymous";
            await _roleService.CancelRoleRequestAsync(requestId, username);
            return Ok(Result.Success());
        }

        [HttpPost("{roleId}/claims")]
        public async Task<IActionResult> AddRoleClaims(long roleId, [FromBody] IEnumerable<ClaimDto> claims)
        {
            await _roleService.AddClaimsAsync(roleId, claims);
            return Ok(Result.Success());
        }

        [HttpDelete("{roleId}/claims")]
        public async Task<IActionResult> RemoveRoleClaim(long roleId, [FromBody] ClaimDto claim)
        {
            await _roleService.RemoveClaimAsync(roleId, claim);
            return Ok(Result.Success());
        }

        [HttpPost("{code}/draft/cancel")]
        public async Task<IActionResult> CancelDraftByCode(string code)
        {
            var username = User.FindFirstValue(Flex.Security.ClaimTypes.Sub) ?? User?.Identity?.Name ?? "anonymous";
            // Tìm bản nháp CREATE theo code
            var pendingRequest = await _roleService.GetDraftCreateRequestByCodeAsync(code, username);
            if (pendingRequest == null)
                return NotFound(Result.Failure(message: "Draft create request not found for this code or you are not the creator."));
            await _roleService.CancelRoleRequestAsync(0, username);
            return Ok(Result.Success());
        }

        

        // ===== XÓA API LẤY DANH SÁCH YÊU CẦU CHỜ DUYỆT (pending-requests) =====
        // [HttpGet("pending-requests")]
        // public async Task<IActionResult> GetPendingRequests([FromQuery] PendingRequestsPagingRequest request)
        // {
        //     var result = await _roleService.GetPendingRequestsAsync(request);
        //     return Ok(Result.Success(result));
        // }

        /// <summary>
        /// So sánh bản chính và bản nháp
        /// </summary>
        [HttpGet("requests/{requestId}/compare")]
        public async Task<IActionResult> GetRoleComparison(long requestId)
        {
            var result = await _roleService.GetRoleComparisonAsync(requestId);
            if (result == null)
                return NotFound(Result.Failure("Request not found or invalid"));
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Lấy bản nháp (pending/draft) của một vai trò cụ thể
        /// </summary>
        [HttpGet("{roleId}/draft")]
        public async Task<IActionResult> GetDraftByRole(long roleId)
        {
            var draft = await _roleService.GetDraftByRoleAsync(roleId);
            if (draft == null)
                return NotFound(Result.Failure("No draft or pending request found for this role."));
            return Ok(Result.Success(draft));
        }         
    }
}
