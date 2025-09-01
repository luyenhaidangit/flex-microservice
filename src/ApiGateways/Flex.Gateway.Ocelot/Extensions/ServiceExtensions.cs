using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using Ocelot.Cache.CacheManager;
using Flex.Shared.Constants;
using Flex.Shared.Extensions;
using Flex.Infrastructure.Swashbuckle;
using Flex.Gateway.Ocelot.Constants;
using Flex.Security;
using Flex.Infrastructure.Json;

namespace Flex.Gateway.Ocelot.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers();
            services.ConfigureJsonOptionsDefault();
            services.AddEndpointsApiExplorer();
            services.ConfigureSwagger();
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
