using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateUserAsync(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpPut("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserDto dto)
        {
            var result = await _userService.UpdateUserAsync(id, dto);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Description == "User not found"))
                {
                    return NotFound();
                }
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpPost("{id:long}/lock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LockUser(long id)
        {
            var success = await _userService.LockUserAsync(id);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost("{id:long}/unlock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnlockUser(long id)
        {
            var success = await _userService.UnlockUserAsync(id);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost("{id:long}/reset-password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(long id, [FromBody] ResetPasswordDto dto)
        {
            var result = await _userService.ResetPasswordAsync(id, dto.NewPassword);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Description == "User not found"))
                {
                    return NotFound();
                }
                return BadRequest(result.Errors);
            }

            return Ok();
        }
    }
}
