using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow;
using Microsoft.AspNetCore.Mvc;

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

        #region Query

        /// <summary>
        /// Get all user with pagination
        /// </summary>
        [HttpGet("approved")]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersPagingRequest request, CancellationToken ct)
        {
            var result = await _userService.GetUsersPagedAsync(request, ct);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get approved user by user name.
        /// </summary>
        [HttpGet("approved/{userName}")]
        public async Task<IActionResult> GetUserByUserName(string userName, CancellationToken ct)
        {
            var result = await _userService.GetUserByUserNameAsync(userName, ct);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get approved user change history by user name.
        /// </summary>
        [HttpGet("approved/{userName}/history")]
        public async Task<IActionResult> GetUserChangeHistory(string userName)
        {
            var result = await _userService.GetUserChangeHistoryAsync(userName);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get all pending user requests with pagination.
        /// </summary>
        [HttpGet("request/pending")]
        public async Task<IActionResult> GetPendingUserRequests([FromQuery] GetUserRequestsPagingRequest request, CancellationToken ct)
        {
            var result = await _userService.GetPendingUserRequestsPagedAsync(request, ct);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get user request pending detail.
        /// </summary>
        [HttpGet("request/pending/{requestId}")]
        public async Task<IActionResult> GetPendingUserRequestById(long requestId)
        {
            var result = await _userService.GetPendingUserRequestByIdAsync(requestId);
            return Ok(Result.Success(result));
        }

        #endregion

        #region Command

        /// <summary>
        /// Create a new user request (requires approval).
        /// </summary>
        [HttpPost("request/create")]
        public async Task<IActionResult> CreateUserRequest([FromBody] CreateUserRequest dto)
        {
            var id = await _userService.CreateUserRequestAsync(dto);
            return Ok(Result.Success(id));
        }

        /// <summary>
        /// Create update user request.
        /// </summary>
        [HttpPost("request/update")]
        public async Task<IActionResult> UpdateUserRequest([FromBody] UpdateUserRequest request)
        {
            var id = await _userService.UpdateUserRequestAsync(request);
            return Ok(Result.Success(id));
        }

        /// <summary>
        /// Create deltete user request.
        /// </summary>
        [HttpPost("request/delete/{userName}")]
        public async Task<IActionResult> DeleteUserRequest(string userName)
        {
            await _userService.DeleteUserRequestAsync(userName);
            return Ok(Result.Success(message: "Delete user success"));
        }

        /// <summary>
        /// Approve pending user request by ID.
        /// </summary>
        [HttpPost("request/pending/{requestId}/approve")]
        public async Task<IActionResult> ApprovePendingUserRequest(long requestId)
        {
            var result = await _userService.ApprovePendingUserRequestAsync(requestId);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Reject pending user request by ID.
        /// </summary>
        [HttpPost("request/pending/{requestId}/reject")]
        public async Task<IActionResult> RejectPendingUserRequest(long requestId, [FromBody] RejectRequest request)
        {
            var result = await _userService.RejectPendingUserRequestAsync(requestId, request.Reason);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Change user password (required on first login).
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _userService.ChangePasswordAsync(request);
            return Ok(Result.Success(result, "Password changed successfully"));
        }

        /// <summary>
        /// Check if user needs to change password on first login.
        /// </summary>
        [HttpGet("{userName}/password-change-required")]
        public async Task<IActionResult> CheckPasswordChangeRequired(string userName)
        {
            var result = await _userService.CheckPasswordChangeRequiredAsync(userName);
            return Ok(Result.Success(result));
        }

        #endregion
    }
}
