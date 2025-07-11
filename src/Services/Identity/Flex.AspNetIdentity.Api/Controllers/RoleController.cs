using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.AspNetIdentity.Api.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [Route("api/roles")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RoleController : ControllerBase
    {
        private IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Lấy danh sách phân trang các vai trò (roles), bao gồm cả các yêu cầu tạo mới đang chờ duyệt.
        /// </summary>
        /// <param name="request">Thông tin phân trang và tìm kiếm</param>
        /// <returns>Danh sách vai trò phân trang</returns>
        [HttpGet]
        public async Task<IActionResult> GetPagingRolesAsync([FromQuery] GetRolesPagingRequest request)
        {
            var result = await _roleService.GetRolePagedAsync(request);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Lấy thông tin chi tiết request để hiển thị trong modal
        /// Trả về oldData và newData để so sánh
        /// </summary>
        [HttpGet("role-requests/{requestId}")]
        public async Task<IActionResult> GetRoleRequestDetail(long requestId)
        {
            var result = await _roleService.GetRoleRequestDetailAsync(requestId);
            if (result == null)
                return NotFound(Result.Failure("Request not found"));
            return Ok(Result.Success(result));
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetRoleById(long id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);
            return Ok(Result.Success(result));
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetRoleByCode(string code)
        {
            var result = await _roleService.GetRoleByCodeAsync(code);
            return Ok(Result.Success(result));
        }

        [HttpPost("requests/create")]
        public async Task<IActionResult> CreateRoleRequest([FromBody] CreateRoleDto dto)
        {
            var username = User.FindFirstValue(Flex.Security.ClaimTypes.Sub) ?? User?.Identity?.Name ?? "anonymous";
            var id = await _roleService.CreateAddRoleRequestAsync(dto, username);
            return Ok(Result.Success(id));
        }

        [HttpPost("{roleId}/requests/update")]
        public async Task<IActionResult> CreateUpdateRoleRequest(long roleId, [FromBody] UpdateRoleDto dto)
        {
            var username = User.FindFirstValue(Flex.Security.ClaimTypes.Sub) ?? User?.Identity?.Name ?? "anonymous";
            var id = await _roleService.CreateUpdateRoleRequestAsync(roleId, dto, username);
            return Ok(Result.Success(id));
        }

        [HttpPost("{roleId}/requests/delete")]
        public async Task<IActionResult> CreateDeleteRoleRequest(long roleId)
        {
            var username = User.FindFirstValue(Flex.Security.ClaimTypes.Sub) ?? User?.Identity?.Name ?? "anonymous";
            var id = await _roleService.CreateDeleteRoleRequestAsync(roleId, username);
            return Ok(Result.Success(id));
        }

        [HttpPost("requests/{requestId}/approve")]
        public async Task<IActionResult> ApproveRoleRequest(long requestId, [FromBody] string? comment)
        {
            await _roleService.ApproveRoleRequestAsync(requestId, comment);
            return Ok(Result.Success());
        }

        [HttpPost("requests/{requestId}/reject")]
        public async Task<IActionResult> RejectRoleRequest(long requestId, [FromBody] string reason)
        {
            await _roleService.RejectRoleRequestAsync(requestId, reason);
            return Ok(Result.Success());
        }

        [HttpPost("requests/{requestId}/cancel")]
        public async Task<IActionResult> CancelRoleRequest(long requestId)
        {
            var username = User.FindFirstValue(Flex.Security.ClaimTypes.Sub) ?? User?.Identity?.Name ?? "anonymous";
            await _roleService.CancelRoleRequestAsync(requestId, username);
            return Ok(Result.Success());
        }

        [HttpGet("{roleId}/claims")]
        public async Task<IActionResult> GetRoleClaims(long roleId)
        {
            var claims = await _roleService.GetClaimsAsync(roleId);
            return Ok(Result.Success(claims));
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
