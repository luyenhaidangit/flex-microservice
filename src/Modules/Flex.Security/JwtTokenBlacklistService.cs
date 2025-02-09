using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Flex.Security
{
    public class JwtTokenBlacklistService : IJwtTokenBlacklistService
    {
        private readonly IDatabase _redisDb;

        public JwtTokenBlacklistService(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisDb = connectionMultiplexer.GetDatabase();
        }

        public JwtSecurityToken CreateJwtSecurityToken(JwtSettings settings, List<Claim> claims)
        {
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.SecretKey)), settings.Algorithm);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                expires: DateTime.UtcNow.AddMinutes(settings.ExpiryInMinutes),
                claims: claims,
                signingCredentials: creds
            );

            return token;
        }

        public string GenerateToken(JwtSettings settings, List<Claim> claims)
        {
            var jwtToken = this.CreateJwtSecurityToken(settings, claims);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return tokenString;
        }

        public async Task<bool> IsTokenRevokedAsync(string jti)
        {
            return await _redisDb.KeyExistsAsync($"{JwtConstants.Redis.RootKey}:{jti}");
        }

        public async Task RevokeTokenAsync(string jti, TimeSpan ttl)
        {
            if (string.IsNullOrEmpty(jti))
            {
                throw new ArgumentException("JTI cannot be null or empty.", nameof(jti));
            }

            await _redisDb.StringSetAsync($"{JwtConstants.Redis.RootKey}:{jti}", "", ttl);
        }
    }
}
