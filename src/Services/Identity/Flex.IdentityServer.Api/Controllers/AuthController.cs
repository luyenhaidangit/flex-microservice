using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Flex.IdentityServer.Api.Entities;
using Flex.Security;
using Flex.Shared.DTOs.Identity;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Flex.IdentityServer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        

        [HttpGet("test")]
        public IActionResult GetTest()
        {
            return Ok(new { message = "API is working fine!" });
        }
    }
}
