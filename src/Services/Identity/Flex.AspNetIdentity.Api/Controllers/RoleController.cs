using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Models.Role;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using static Flex.AspNetIdentity.Api.Services.RoleService;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [Route("api/roles")]
    [ApiController]
    //[Authorize(Policy = Permissions.Branch.Create)]
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
        public async Task<IActionResult> GetPagingApprovedRoles([FromQuery] GetApproveRolesPagingRequest request)
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
        public async Task<IActionResult> GetPagingPendingRolesAsync([FromQuery] GetApproveRolesPagingRequest request)
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
            try
            {
                // ===== Validation =====
                if (requestId <= 0)
                {
                    return BadRequest(Result.Failure("RequestId must be greater than 0."));
                }

                // ===== Process approval =====
                var result = await _roleService.ApprovePendingRoleRequestAsync(requestId, dto?.Comment);
                
                // ===== Return result =====
                return Ok(Result.Success(result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(Result.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result.Failure($"Failed to approve role request: {ex.Message}"));
            }
        }

        /// <summary>
        /// Reject pending role request by ID.
        /// </summary>
        [HttpPost("pending/{requestId}/reject")]
        public async Task<IActionResult> RejectPendingRoleRequest(long requestId, [FromBody] RejectRoleRequestDto? dto = null)
        {
            var result = await _roleService.RejectPendingRoleRequestAsync(requestId, dto?.Reason);
            return Ok(Result.Success(result));
        }

        [HttpGet("tree-permissions")]
        public async Task<IActionResult> GetTreePermissions([FromQuery] string? code, CancellationToken ct)
        {
            var result = await _roleService.GetPermissionFlagsAsync(code, ct);
            return Ok(Result.Success(result));
        }

        [HttpPut("{code}/permissions")]
        public async Task<IActionResult> SavePermissions(string code, [FromBody] SavePermissionsRequest req, CancellationToken ct)
        {
            await _roleService.UpdateRolePermissionsAsync(code, req.PermissionCodes, ct);
            return NoContent();
        }
    }
}
