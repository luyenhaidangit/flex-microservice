using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.Auth;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.Security;
using Flex.Infrastructure.Exceptions;
using Flex.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ClaimTypesApp = Flex.Security.ClaimTypes;
using ClaimTypesAsp = System.Security.Claims.ClaimTypes;

namespace Flex.AspNetIdentity.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IdentityDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenBlacklistService _jwtBacklistTokenService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IdentityDbContext dbContext,
            IPasswordHasher<User> passwordHasher,
            IJwtTokenBlacklistService jwtBacklistTokenService,
            IOptions<JwtSettings> jwtSettings,
            IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _jwtBacklistTokenService = jwtBacklistTokenService;
            _jwtSettings = jwtSettings.Value;
            _userRepository = userRepository;
        }

        public async Task<LoginResult> LoginAsync(LoginByUserNameRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
            if (user is null)
            {
                throw new ValidationException(ErrorCode.InvalidCredentials);
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new ValidationException(ErrorCode.InvalidCredentials);
            }

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verify == PasswordVerificationResult.Failed)
            {
                throw new ValidationException(ErrorCode.InvalidCredentials);
            }

            var roleNames = await _userRepository.GetRoleNamesAsync(user.Id, cancellationToken);

            // Include standard claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypesApp.Jti,  Guid.NewGuid().ToString()),
                new Claim(ClaimTypesApp.Iss, _jwtSettings.Issuer),
                new Claim(ClaimTypesApp.Aud, _jwtSettings.Audience),
                new Claim(ClaimTypesApp.Sub, user.UserName ?? string.Empty),
                new Claim(ClaimTypesApp.Email, user.Email ?? string.Empty),
            };

            // Include role claims
            claims.AddRange(roleNames.Select(role => new Claim(ClaimTypesAsp.Role, role)));

            var token = _jwtBacklistTokenService.GenerateToken(_jwtSettings, claims);
            var result = new LoginResult(token);

            return result;
        }

        public async Task<bool> LogoutAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
        {
            var jti = user.FindFirstValue(ClaimTypesApp.Jti);
            var expClaim = user.FindFirstValue(ClaimTypesApp.Exp);

            if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expClaim) || !long.TryParse(expClaim, out var expUnix))
            {
                return false;
            }

            var exp = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            var now = DateTime.UtcNow;

            if (exp <= now)
            {
                return true;
            }

            var ttl = exp - now;
            await _jwtBacklistTokenService.RevokeTokenAsync(jti, ttl);
            return true;
        }

        public async Task<UserInfoResult> GetCurrentUserInfoAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
        {
            var userName = user.FindFirstValue(ClaimTypesApp.Sub);
            if (string.IsNullOrEmpty(userName))
            {
                throw new ValidationException(ErrorCode.Unauthorized);
            }

            var entity = await _userRepository.GetByUserNameAsync(userName, cancellationToken)
                ?? throw new ValidationException(ErrorCode.UserNotFound);

            var userInfo = new UserInfoResult
            {
                UserName = entity.UserName ?? string.Empty,
                Email = entity.Email ?? string.Empty
            };

            return userInfo;
        }
    }
}
