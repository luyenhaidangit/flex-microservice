using AutoMapper;
using Flex.AspNetIdentity.Api.Entities;
using Flex.Security;
using Flex.Shared.DTOs.Identity;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ClaimTypesApp = Flex.Security.ClaimTypes;
using ClaimTypesAsp = System.Security.Claims.ClaimTypes;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenBlacklistService _jwtBacklistTokenService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<User> userManager, IJwtTokenBlacklistService jwtBacklistTokenService, IOptions<JwtSettings> jwtSettings)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _jwtBacklistTokenService = jwtBacklistTokenService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            _logger.LogInformation("Attempting to register user: {Email}", request.Email);

            // Validate
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(Result.Failure(message: "User already exists with this email."));
            }

            var existingUsername = await _userManager.FindByNameAsync(request.Email);
            if (existingUsername != null)
            {
                return BadRequest(Result.Failure(message: "User already exists with this username."));
            }

            // Process
            var user = _mapper.Map<User>(request);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(Result.Failure(message: "User registration failed.", errors: result.Errors));
            }

            _logger.LogInformation("User {Email} registered successfully.", request.Email);
            return Ok(Result.Success(message: "User registered successfully"));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginByUserNameRequest request)
        {
            _logger.LogInformation("User {Username} attempting to login.", request.UserName);

            // Validate
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null)
            {
                return BadRequest(Result.Failure(message: "User not exists with this username."));
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return BadRequest(Result.Failure(message: "Invalid username or password."));
            }

            // Process
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypesApp.Jti,  Guid.NewGuid().ToString()),
                new Claim(ClaimTypesApp.Iss, _jwtSettings.Issuer),
                new Claim(ClaimTypesApp.Aud, _jwtSettings.Audience),
                new Claim(ClaimTypesApp.Sub, user.UserName ?? string.Empty),
                new Claim(ClaimTypesApp.Email, user.Email ?? string.Empty),
            };

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypesAsp.Role, role)));

            var token = _jwtBacklistTokenService.GenerateToken(_jwtSettings, claims);

            var result = new LoginResult(token);

            _logger.LogInformation("User {Username} logged in successfully.", request.UserName);
            return Ok(Result.Success(message: "Login success!", data: result));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var jti = User.FindFirstValue(ClaimTypesApp.Jti);
            var expClaim = User.FindFirstValue(ClaimTypesApp.Exp);

            if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expClaim) || !long.TryParse(expClaim, out var expUnix))
            {
                return BadRequest(Result.Failure("Invalid token."));
            }

            var exp = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            var now = DateTime.UtcNow;

            if (exp <= now)
            {
                return Ok(Result.Success("Token already expired, no need to blacklist."));
            }

            var ttl = exp - now;

            await _jwtBacklistTokenService.RevokeTokenAsync(jti, ttl);
            _logger.LogInformation("User {UserName} logged out, token revoked.", User.FindFirstValue(ClaimTypesApp.Sub));

            return Ok(Result.Success(message: "Logout success!"));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var userName = User.FindFirstValue(ClaimTypesApp.Sub);

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(Result.Failure(message: "Unauthorized"));
            }

            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
            {
                return BadRequest(Result.Failure(message: "User not found"));
            }

            var userInfo = new UserInfoResult
            {
                UserName = user.UserName,
                Email = user.Email
            };

            return Ok(Result.Success(message: "Get info user success!", data: userInfo));
        }
    }
}
