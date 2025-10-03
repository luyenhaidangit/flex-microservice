using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.Auth;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ClaimTypesApp = Flex.Security.ClaimTypes;
using ClaimTypesAsp = System.Security.Claims.ClaimTypes;

namespace Flex.AspNetIdentity.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IdentityDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenBlacklistService _jwtBacklistTokenService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IdentityDbContext dbContext,
            IPasswordHasher<User> passwordHasher,
            IJwtTokenBlacklistService jwtBacklistTokenService,
            IOptions<JwtSettings> jwtSettings)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _jwtBacklistTokenService = jwtBacklistTokenService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<LoginResult?> LoginAsync(LoginByUserNameRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);
            if (user is null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return null;
            }

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verify == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var roleNames = await _dbContext.Set<UserRole>()
                .Where(ur => ur.UserId == user.Id)
                .Join(_dbContext.Set<Role>(), ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
                .Distinct()
                .ToListAsync(cancellationToken);

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

        public async Task<UserInfoResult?> GetCurrentUserInfoAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
        {
            var userName = user.FindFirstValue(ClaimTypesApp.Sub);
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }

            var entity = await _dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
            if (entity is null)
            {
                return null;
            }

            var userInfo = new UserInfoResult
            {
                UserName = entity.UserName,
                Email = entity.Email
            };

            return userInfo;
        }
    }
}
