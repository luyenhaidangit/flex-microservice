using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    { 
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Get all user with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersPagingRequest request, CancellationToken ct)
        {
            var result = await _userService.GetUsersPagedAsync(request, ct);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get all pending user requests with pagination
        /// </summary>
        [HttpGet("requests/pending")]
        public async Task<IActionResult> GetPendingUserRequests([FromQuery] GetUserRequestsPagingRequest request, CancellationToken ct)
        {
            var result = await _userService.GetPendingUserRequestsPagedAsync(request, ct);
            return Ok(Result.Success(result));
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetUser(string userName, CancellationToken ct)
        {
            var result = await _userService.GetUserByUserNameAsync(userName, ct);
            return Ok(Result.Success(result));
        }

        [HttpGet("{userName}/history")]
        public async Task<IActionResult> GetUserChangeHistory(string userName)
        {
            var result = await _userService.GetUserChangeHistoryAsync(userName);
            return Ok(Result.Success(result));
        }

        [HttpPut("{userName}/roles")]
        [Authorize(Policy = "USERS.ASSIGN_ROLE")]
        public async Task<IActionResult> AssignRoles(string userName, [FromBody] AssignRolesRequest req, CancellationToken ct)
        {
            await _userService.AssignRolesAsync(userName, req.RoleCodes, ct);
            return Ok(Result.Success(message: "Assign roles success"));
        }

        [HttpPost("{userName}/lock")]
        [Authorize(Policy = "USERS.LOCK")]
        public async Task<IActionResult> LockUser(string userName)
        {
            await _userService.LockAsync(userName);
            return Ok(Result.Success(message: "Lock user success"));
        }

        [HttpPost("{userName}/unlock")]
        [Authorize(Policy = "USERS.UNLOCK")]
        public async Task<IActionResult> UnlockUser(string userName)
        {
            await _userService.UnlockAsync(userName);
            return Ok(Result.Success(message: "Unlock user success"));
        }

        [HttpPost("{userName}/reset-password")]
        [Authorize(Policy = "USERS.RESET_PW")]
        public async Task<IActionResult> ResetPassword(string userName)
        {
            var token = await _userService.ResetPasswordAsync(userName);
            return Ok(Result.Success(token));
        }

        [HttpPost("requests/create")]
        [Authorize(Policy = "USERS.CREATE")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDto dto)
        {
            var id = await _userService.CreateUserAsync(dto);
            return Ok(Result.Success(id));
        }

        [HttpPost("{userName}/update")]
        [Authorize(Policy = "USERS.UPDATE")]
        public async Task<IActionResult> Update(string userName, [FromBody] UpdateUserRequestDto dto)
        {
            await _userService.UpdateUserAsync(userName, dto);
            return Ok(Result.Success(message: "Update user success"));
        }

        [HttpPost("{userName}/delete")]
        [Authorize(Policy = "USERS.DELETE")]
        public async Task<IActionResult> Delete(string userName, [FromBody] DeleteUserRequestDto dto)
        {
            await _userService.DeleteUserAsync(userName, dto);
            return Ok(Result.Success(message: "Delete user success"));
        }

        /// <summary>
        /// Get user request pending detail.
        /// </summary>
        [HttpGet("requests/pending/{requestId}")]
        public async Task<IActionResult> GetPendingUserRequestById(long requestId)
        {
            var result = await _userService.GetPendingUserRequestByIdAsync(requestId);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Approve pending user request by ID.
        /// </summary>
        [HttpPost("requests/pending/{requestId}/approve")]
        public async Task<IActionResult> ApprovePendingUserRequest(long requestId, [FromBody] ApproveUserRequestDto? dto = null)
        {
            try
            {
                // ===== Validation =====
                if (requestId <= 0)
                {
                    return BadRequest(Result.Failure("RequestId must be greater than 0."));
                }

                // ===== Process approval =====
                var result = await _userService.ApprovePendingUserRequestAsync(requestId, dto?.Comment);
                
                // ===== Return result =====
                return Ok(Result.Success(result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(Result.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result.Failure($"Failed to approve user request: {ex.Message}"));
            }
        }

        /// <summary>
        /// Reject pending user request by ID.
        /// </summary>
        [HttpPost("requests/pending/{requestId}/reject")]
        public async Task<IActionResult> RejectPendingUserRequest(long requestId, [FromBody] RejectUserRequestDto? dto = null)
        {
            try
            {
                // ===== Validation =====
                if (requestId <= 0)
                {
                    return BadRequest(Result.Failure("RequestId must be greater than 0."));
                }

                // ===== Process rejection =====
                var result = await _userService.RejectPendingUserRequestAsync(requestId, dto?.Reason);
                
                // ===== Return result =====
                return Ok(Result.Success(result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(Result.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result.Failure($"Failed to reject user request: {ex.Message}"));
            }
        }
    }
}
