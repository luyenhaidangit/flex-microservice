using AutoMapper;
using Flex.AspNetIdentity.Api.Models.Auth;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClaimTypesApp = Flex.Security.ClaimTypes;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly Services.Interfaces.IAuthService _authService;

        public AuthController(
            ILogger<AuthController> logger,
            IMapper mapper,
            Services.Interfaces.IAuthService authService)
        {
            _logger = logger;
            _mapper = mapper;
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginByUserNameRequest request)
        {
            var login = await _authService.LoginAsync(request, HttpContext.RequestAborted);
            if (login is null)
            {
                return BadRequest(Result.Failure(message: "Invalid username or password."));
            }

            return Ok(Result.Success(message: "Login success!", data: login));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var ok = await _authService.LogoutAsync(User, HttpContext.RequestAborted);
            _logger.LogInformation("User {UserName} logged out.", User.FindFirstValue(ClaimTypesApp.Sub));
            if (!ok)
            {
                return BadRequest(Result.Failure("Invalid token."));
            }
            return Ok(Result.Success(message: "Logout success!"));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var userInfo = await _authService.GetCurrentUserInfoAsync(User, HttpContext.RequestAborted);
            if (userInfo is null)
            {
                if (string.IsNullOrEmpty(User.FindFirstValue(ClaimTypesApp.Sub)))
                {
                    return Unauthorized(Result.Failure(message: "Unauthorized"));
                }
                return BadRequest(Result.Failure(message: "User not found"));
            }
            return Ok(Result.Success(message: "Get info user success!", data: userInfo));
        }
    }
}
