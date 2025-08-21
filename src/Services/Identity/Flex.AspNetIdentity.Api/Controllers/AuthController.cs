using AutoMapper;
using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.Security;
using Flex.Shared.DTOs.Identity;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ClaimTypesApp = Flex.Security.ClaimTypes;
using ClaimTypesAsp = System.Security.Claims.ClaimTypes;

namespace Flex.AspNetIdentity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly IdentityDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenBlacklistService _jwtBacklistTokenService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(
            ILogger<AuthController> logger,
            IMapper mapper,
            IdentityDbContext dbContext,
            IPasswordHasher<User> passwordHasher,
            IJwtTokenBlacklistService jwtBacklistTokenService,
            IOptions<JwtSettings> jwtSettings)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _jwtBacklistTokenService = jwtBacklistTokenService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginByUserNameRequest request)
        {
            _logger.LogInformation("User {Username} attempting to login.", request.UserName);

            // Validate
            var user = await _dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user is null)
            {
                return BadRequest(Result.Failure(message: "User not exists with this username."));
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest(Result.Failure(message: "User has no password set."));
            }

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verify == PasswordVerificationResult.Failed)
            {
                return BadRequest(Result.Failure(message: "Invalid username or password."));
            }

            // Load roles via join
            var roleNames = await _dbContext.Set<UserRole>()
                .Where(ur => ur.UserId == user.Id)
                .Join(_dbContext.Set<Role>(), ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
                .Distinct()
                .ToListAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypesApp.Jti,  Guid.NewGuid().ToString()),
                new Claim(ClaimTypesApp.Iss, _jwtSettings.Issuer),
                new Claim(ClaimTypesApp.Aud, _jwtSettings.Audience),
                new Claim(ClaimTypesApp.Sub, user.UserName ?? string.Empty),
                new Claim(ClaimTypesApp.Email, user.Email ?? string.Empty),
            };

            claims.AddRange(roleNames.Select(role => new Claim(ClaimTypesAsp.Role, role)));

            var token = _jwtBacklistTokenService.GenerateToken(_jwtSettings, claims);
            var result = new LoginResult(token);

            _logger.LogInformation("User {Username} logged in successfully.", request.UserName);
            return Ok(Result.Success(message: "Login success!", data: result));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            _logger.LogInformation("Attempting to register user: {Email}", request.Email);

            // Validate
            var existingUser = await _dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return BadRequest(Result.Failure(message: "User already exists with this email."));
            }

            var existingUsername = await _dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.UserName == request.Email);
            if (existingUsername != null)
            {
                return BadRequest(Result.Failure(message: "User already exists with this username."));
            }

            // Process
            var user = _mapper.Map<User>(request);
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _dbContext.Set<User>().AddAsync(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("User {Email} registered successfully.", request.Email);
            return Ok(Result.Success(message: "User registered successfully"));
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

            var user = await _dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.UserName == userName);

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
