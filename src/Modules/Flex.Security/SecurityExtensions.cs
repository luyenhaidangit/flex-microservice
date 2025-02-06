using Microsoft.Extensions.DependencyInjection;

namespace Flex.Security
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddJwtTokenSecurity(this IServiceCollection services)
        {
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}
