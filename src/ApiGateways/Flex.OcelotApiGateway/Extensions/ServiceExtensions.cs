using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using Ocelot.Cache.CacheManager;
using Flex.Shared.Constants;
using Flex.Shared.Options;
using Flex.Shared.Extensions;
using Flex.Infrastructure.Swashbuckle;
using Flex.OcelotApiGateway.Constants;
using Flex.Security;

namespace Flex.OcelotApiGateway.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ApiConfiguration>().Bind(configuration.GetSection(ConfigurationConstants.ApiConfiguration)).ValidateDataAnnotations().ValidateOnStart();
            services.AddOptions<Flex.Security.JwtSettings>().Bind(configuration.GetSection(ConfigurationConstants.JwtSettings)).ValidateDataAnnotations().ValidateOnStart();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind configuration settings
            var apiConfiguration = configuration.GetRequiredSection<ApiConfiguration>(ConfigurationConstants.ApiConfiguration);

            // Add services to the container.
            services.AddControllers().ApplyJsonSettings();

            services.AddEndpointsApiExplorer();

            services.ConfigureSwagger(apiConfiguration);

            services.ConfigureRouteOptions();
            services.ConfigureValidationErrorResponse();

            // Register services DI container
            services.AddInfrastructureServices();

            // Configure Ocelot API gateway
            services.ConfigureOcelot(configuration);

            // Add Jwt authentication
            services.AddAuthenticationJwtToken(configuration);

            // Configure Cors
            services.ConfigureCors(configuration);

            return services;
        }

        #region Infrastructure
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services;
        }

        private static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOcelot(configuration)
                    .AddPolly()
                    .AddCacheManager(x => x.WithDictionaryHandle());

            services.AddSwaggerForOcelot(configuration, x =>
            {
                x.GenerateDocsForGatewayItSelf = false;
            });
        }

        //private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var settings = configuration.GetSection(ConfigurationConstants.JwtSettings).Get<JwtSettings>();

        //    if (settings == null || string.IsNullOrEmpty(settings.SecretKey))
        //    {
        //        throw new ArgumentNullException($"{nameof(JwtSettings)} is not configured properly");
        //    }

        //    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));

        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = signingKey,
        //        ValidateIssuer = false,
        //        ValidateAudience = false,
        //        ValidateLifetime = false,
        //        ClockSkew = TimeSpan.Zero,
        //        RequireExpirationTime = false
        //    };
        //    services.AddAuthentication(o =>
        //    {
        //        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    }).AddJwtBearer(x =>
        //    {
        //        x.SaveToken = true;
        //        x.RequireHttpsMetadata = false;
        //        x.TokenValidationParameters = tokenValidationParameters;
        //    });

        //    return services;
        //}

        private static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var origins = configuration.GetRequiredValue<string>(ConfigurationConstants.AllowOrigins);

            services.AddCors(options =>
            {
                options.AddPolicy(GatewayConstants.CorsPolicy, buider =>
                {
                    buider.WithOrigins(origins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }
        #endregion
    }
}
