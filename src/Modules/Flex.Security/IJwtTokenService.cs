using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Flex.Security
{
    public interface IJwtTokenService
    {
        JwtSecurityToken CreateJwtSecurityToken(JwtSettings settings, List<Claim> claims);

        string GenerateToken(JwtSettings settings, List<Claim> claims);
    }
}
