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
    [Route("api/identity")]
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
            // Validate
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(Result.Failure("User already exists with this email."));
            }

            var existingUsername = await _userManager.FindByNameAsync(request.Email);
            if (existingUsername != null)
            {
                return BadRequest(Result.Failure("User already exists with this username."));
            }

            // Process
            var user = _mapper.Map<User>(request);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(Result.Failure(message: "User registration failed.", errors: result.Errors));
            }

            return Ok(Result.Success("User registered successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginByUserNameRequest request)
        {
            // Validate
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null)
            {
                return BadRequest(Result.Failure("User not exists with this username."));
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return BadRequest(Result.Failure("Invalid username or password."));
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

            return Ok(Result.Success(message: "Login success!",data: result));
        }
    }
}
