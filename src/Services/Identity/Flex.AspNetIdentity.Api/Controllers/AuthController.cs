using AutoMapper;
using Flex.AspNetIdentity.Api.Entities;
using Flex.Security;
using Flex.Shared.DTOs.Identity;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ClaimTypesApp = Flex.Security.ClaimTypes;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<User> userManager, IJwtTokenService jwtTokenService, IOptions<JwtSettings> jwtSettings)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypesApp.UserName, user.UserName ?? string.Empty),
                new Claim(ClaimTypesApp.Email, user.Email ?? string.Empty),
            };

            var token = _jwtTokenService.GenerateToken(_jwtSettings, claims);

            var result = new LoginResult(token);

            _logger.LogInformation("User {Username} logged in successfully.", request.UserName);
            return Ok(Result.Success(message: "Login success!",data: result));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var userName = User.FindFirstValue(ClaimTypesApp.UserName);

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

            return Ok(Result.Success(message: "Login success!", data: userInfo));
        }
    }
}
