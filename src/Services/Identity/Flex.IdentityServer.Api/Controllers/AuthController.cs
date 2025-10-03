//using AutoMapper;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Flex.IdentityServer.Api.Entities;
//using Flex.AspNetIdentity.Api.Models.Auth;
//using Flex.Shared.SeedWork;
//using Microsoft.AspNetCore.Authorization;
//using Duende.IdentityModel.Client;
//using System.Security.Claims;
//using Flex.Security;
//using ClaimTypesApp = Flex.Security.ClaimTypes;

//namespace Flex.IdentityServer.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly ILogger<AuthController> _logger;
//        private readonly IMapper _mapper;
//        private readonly UserManager<User> _userManager;
//        private readonly HttpClient _httpClient;
//        private readonly IConfiguration _configuration;
//        private readonly IJwtTokenBlacklistService _jwtBacklistTokenService;

//        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<User> userManager, IHttpClientFactory httpClientFactory, IConfiguration configuration, IJwtTokenBlacklistService jwtBacklistTokenService)
//        {
//            _logger = logger;
//            _mapper = mapper;
//            _userManager = userManager;
//            _httpClient = httpClientFactory.CreateClient();
//            _configuration = configuration;
//            _jwtBacklistTokenService = jwtBacklistTokenService;
//        }

//        [HttpPost("login")]
//        [AllowAnonymous]
//        public async Task<IActionResult> Login([FromBody] LoginByUserNameRequest request)
//        {
//            _logger.LogInformation("User {Username} attempting to login.", request.UserName);

//            // Validate
//            var user = await _userManager.FindByNameAsync(request.UserName);

//            if (user is null)
//            {
//                return BadRequest(Result.Failure(message: "User not exists with this username."));
//            }

//            if (!await _userManager.CheckPasswordAsync(user, request.Password))
//            {
//                return BadRequest(Result.Failure(message: "Invalid username or password."));
//            }

//            // Process
//            var tokenEndpoint = _configuration["IdentityServer:TokenEndpoint"];

//            var tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
//            {
//                Address = tokenEndpoint,
//                ClientId = "server_client",
//                ClientSecret = "server_secret",
//                UserName = request.UserName,
//                Password = request.Password,
//                Scope = "openid profile roles my_api.full_access"
//            });

//            if (tokenResponse.IsError)
//            {
//                return BadRequest(Result.Failure(message: tokenResponse.Error));
//            }

//            var result = new LoginResult(tokenResponse.AccessToken);

//            _logger.LogInformation("User {Username} logged in successfully.", request.UserName);
//            return Ok(Result.Success(message: "Login success!", data: result));
//        }

//        [HttpPost("logout")]
//        public async Task<IActionResult> Logout()
//        {
//            var jti = User.FindFirstValue(ClaimTypesApp.Jti);
//            var expClaim = User.FindFirstValue(ClaimTypesApp.Exp);

//            if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expClaim) || !long.TryParse(expClaim, out var expUnix))
//            {
//                return BadRequest(Result.Failure("Invalid token."));
//            }

//            var exp = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
//            var now = DateTime.UtcNow;

//            if (exp <= now)
//            {
//                return Ok(Result.Success("Token already expired, no need to blacklist."));
//            }

//            var ttl = exp - now;

//            await _jwtBacklistTokenService.RevokeTokenAsync(jti, ttl);
//            _logger.LogInformation("User {UserName} logged out, token revoked.", User.FindFirstValue(ClaimTypesApp.Sub));

//            return Ok(Result.Success(message: "Logout success!"));
//        }

//        [HttpGet("me")]
//        public async Task<IActionResult> GetCurrentUserInfo()
//        {
//            var userName = User.FindFirstValue(ClaimTypesApp.Sub);

//            if (string.IsNullOrEmpty(userName))
//            {
//                return Unauthorized(Result.Failure(message: "Unauthorized"));
//            }

//            var user = await _userManager.FindByNameAsync(userName);

//            if (user is null)
//            {
//                return BadRequest(Result.Failure(message: "User not found"));
//            }

//            var userInfo = new UserInfoResult
//            {
//                UserName = user.UserName,
//                Email = user.Email
//            };

//            return Ok(Result.Success(message: "Get info user success!", data: userInfo));
//        }
//    }
//}
