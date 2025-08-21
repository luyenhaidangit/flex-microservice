using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    { 
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userAdminService)
        {
            _logger = logger;
            _userService = userAdminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersPagingRequest request, CancellationToken ct)
        {
            var result = await _userService.GetUsersPagedAsync(request, ct);
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
    }
}
