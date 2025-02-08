using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Flex.Security
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddJwtTokenSecurity(this IServiceCollection services)
        {
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            return services;
        }

        public static IServiceCollection AddAuthenticationJwtToken(this IServiceCollection services, JwtSettings jwtSettings)
        {
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                        };
                    });

            return services;
        }
    }
}
