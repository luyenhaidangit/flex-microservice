namespace Flex.Security
{
    public interface IJwtTokenBlacklistService : IJwtTokenService
    {
        Task RevokeTokenAsync(string jti, TimeSpan ttl);

        Task<bool> IsTokenRevokedAsync(string jti);
    }
}
