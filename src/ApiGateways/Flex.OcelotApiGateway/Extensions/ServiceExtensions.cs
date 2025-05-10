using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using Ocelot.Cache.CacheManager;
using Flex.Shared.Constants;
using Flex.Shared.Options;
using Flex.Shared.Extensions;
using Flex.Infrastructure.Swashbuckle;
using Flex.OcelotApiGateway.Constants;
using Flex.Security;
using Flex.OcelotApiGateway.Models;

namespace Flex.OcelotApiGateway.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ApiConfiguration>().Bind(configuration.GetSection(ConfigKeyConstants.ApiConfiguration)).ValidateDataAnnotations().ValidateOnStart();
            services.AddOptions<Security.JwtSettings>().Bind(configuration.GetSection(ConfigKeyConstants.JwtSettings)).ValidateDataAnnotations().ValidateOnStart();
            services.AddOptions<JwtSchemeSettings>(GatewayConstants.AuthenticationProviderKey.AdminPortal)
            .Bind(configuration.GetSection($"{ConfigKeyConstants.AuthenticationSchemes}:{GatewayConstants.AuthenticationProviderKey.AdminPortal}"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind configuration settings
            var apiConfiguration = configuration.GetRequiredSection<ApiConfiguration>(ConfigKeyConstants.ApiConfiguration);
            var jwtSettingsAdmin = configuration
                .GetSection($"{ConfigKeyConstants.AuthenticationSchemes}:{GatewayConstants.AuthenticationProviderKey.AdminPortal}")
                .Get<JwtSchemeSettings>()!;

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
            //services.AddAuthentication()
            //        .AddJwtBearer(GatewayConstants.AuthenticationProviderKey.AdminPortal, options => {
            //            options.Authority = jwtSettingsAdmin.Authority;
            //            options.Audience = jwtSettingsAdmin.Audience;
            //        });

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

        private static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var origins = configuration.GetRequiredValue<string>(ConfigKeyConstants.AllowOrigins);

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
