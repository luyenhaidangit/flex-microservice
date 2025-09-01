using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Flex.Shared.Extensions;
using Flex.Shared.Constants;
using Flex.Infrastructure.Redis;

namespace Flex.Security
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddJwtTokenSecurity(this IServiceCollection services, IConfiguration configuration, bool isCheckBlacklist = true)
        {
            var redisConnectionString = configuration.GetConnectionString(RedisConstants.RedisConnectionKey) ?? string.Empty;

            if (isCheckBlacklist)
            {
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnectionString));

                services.AddSingleton<IJwtTokenBlacklistService, JwtTokenBlacklistService>();
            } 
            else
            {
                services.AddSingleton<IJwtTokenService, JwtTokenService>();
            }

            return services;
        }

        public static IServiceCollection AddAuthenticationJwtToken(this IServiceCollection services, IConfiguration configuration, bool isCheckBlacklist = true)
        {
            services.AddOptions<JwtSettings>().Bind(configuration.GetSection(ConfigKeyConstants.JwtSettings)).ValidateDataAnnotations().ValidateOnStart();

            var jwtSettings = configuration.GetRequiredSection<JwtSettings>(ConfigKeyConstants.JwtSettings);
            var redisConnectionString = configuration.GetConnectionString(RedisConstants.RedisConnectionKey) ?? string.Empty;

            if (isCheckBlacklist)
            {
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnectionString));

                services.AddSingleton<IJwtTokenBlacklistService, JwtTokenBlacklistService>();

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
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
                    options.MapInboundClaims = false;
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
