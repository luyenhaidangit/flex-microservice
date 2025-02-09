using Flex.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace Flex.Security
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddJwtTokenSecurity(this IServiceCollection services, IConfiguration configuration, bool isCheckBlacklist = true)
        {
            if (isCheckBlacklist)
            {
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString(RedisConstants.RedisConnectionKey)));

                services.AddSingleton<IJwtTokenBlacklistService, JwtTokenBlacklistService>();
            } 
            else
            {
                services.AddSingleton<IJwtTokenService, JwtTokenService>();
            }

            return services;
        }

        public static IServiceCollection AddAuthenticationJwtToken(this IServiceCollection services, JwtSettings jwtSettings, bool isCheckBlacklist = true)
        {
            if(isCheckBlacklist)
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

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var tokenService = context.HttpContext.RequestServices.GetRequiredService<IJwtTokenBlacklistService>();

                            var jti = context.Principal?.FindFirst(ClaimTypes.Jti)?.Value;
                            if (!string.IsNullOrEmpty(jti))
                            {
                                bool isRevoked = await tokenService.IsTokenRevokedAsync(jti);
                                if (isRevoked)
                                {
                                    throw new UnauthorizedAccessException("Token is revoked.");
                                }
                            }
                        }
                    };
                });
            }
            else
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
            }

            return services;
        }
    }
}
