using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    { 
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userAdminService;

        public UserController(UserManager<User> userManager, ILogger<AuthController> logger, IUserService userAdminService)
        {
            _userManager = userManager;
            _logger = logger;
            _userAdminService = userAdminService;
        }

        [HttpGet("approved")]
        //[Authorize(Policy = "USERS.VIEW")]
        public async Task<IActionResult> GetApprovedUsers([FromQuery] GetUsersPagingRequest request, CancellationToken cancellationToken)
        {
            var result = await _userAdminService.GetApprovedUsersPagedAsync(request, cancellationToken);
            return Ok(Result.Success(result));
        }

        [HttpGet("approved/{userName}")]
        [Authorize(Policy = "USERS.VIEW")]
        public async Task<IActionResult> GetApprovedUser(string userName)
        {
            var result = await _userAdminService.GetApprovedUserByUserNameAsync(userName);
            return Ok(Result.Success(result));
        }

        [HttpPut("approved/{userName}/roles")]
        [Authorize(Policy = "USERS.ASSIGN_ROLE")]
        public async Task<IActionResult> AssignRoles(string userName, [FromBody] AssignRolesRequest req, CancellationToken ct)
        {
            await _userAdminService.AssignRolesAsync(userName, req.RoleCodes, ct);
            return NoContent();
        }

        [HttpPost("approved/{userName}/lock")]
        [Authorize(Policy = "USERS.LOCK")]
        public async Task<IActionResult> LockUser(string userName)
        {
            await _userAdminService.LockAsync(userName);
            return NoContent();
        }

        [HttpPost("approved/{userName}/unlock")]
        [Authorize(Policy = "USERS.UNLOCK")]
        public async Task<IActionResult> UnlockUser(string userName)
        {
            await _userAdminService.UnlockAsync(userName);
            return NoContent();
        }

        [HttpPost("approved/{userName}/reset-password")]
        [Authorize(Policy = "USERS.RESET_PW")]
        public async Task<IActionResult> ResetPassword(string userName)
        {
            var token = await _userAdminService.ResetPasswordAsync(userName);
            return Ok(Result.Success(token));
        }

        [HttpPost("requests/create")]
        [Authorize(Policy = "USERS.CREATE")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDto dto)
        {
            var id = await _userAdminService.CreateUserAsync(dto);
            return Ok(Result.Success(id));
        }

        [HttpPost("approved/{userName}/update")]
        [Authorize(Policy = "USERS.UPDATE")]
        public async Task<IActionResult> Update(string userName, [FromBody] UpdateUserRequestDto dto)
        {
            await _userAdminService.UpdateUserAsync(userName, dto);
            return NoContent();
        }

        [HttpPost("approved/{userName}/delete")]
        [Authorize(Policy = "USERS.DELETE")]
        public async Task<IActionResult> Delete(string userName, [FromBody] DeleteUserRequestDto dto)
        {
            await _userAdminService.DeleteUserAsync(userName, dto);
            return NoContent();
        }
    }
}
