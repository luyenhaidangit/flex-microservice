using Flex.Shared.Constants;
using Flex.Shared.Configurations;
using Flex.Shared.Extensions;

namespace Flex.AspNetIdentity.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ApiConfiguration>().Bind(configuration.GetSection(ConfigurationConstants.ApiConfigurationSection)).ValidateDataAnnotations().ValidateOnStart();
            services.AddOptions<JwtSettings>().Bind(configuration.GetSection(ConfigurationConstants.JwtSettingsSection)).ValidateDataAnnotations().ValidateOnStart();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind configuration settings
            var apiConfiguration = configuration.GetRequiredSection<ApiConfiguration>(ConfigurationConstants.ApiConfigurationSection);

            // Add services to the container.
            services.AddControllers().ApplyJsonSettings();

            services.AddEndpointsApiExplorer();

            services.AddOpenApi();

            services.ConfigureRouteOptions();
            services.ConfigureValidationErrorResponse();

            // Register services DI container
            services.AddInfrastructureServices();

            return services;
        }

        #region Infrastructure
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services;
        }
        #endregion
    }
}
