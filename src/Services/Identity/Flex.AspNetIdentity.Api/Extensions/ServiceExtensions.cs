using Flex.Shared.Constants;
using Flex.Shared.Configurations;
using Flex.Shared.Extensions;
using Flex.EntityFrameworkCore.Oracle;
using Flex.AspNetIdentity.Api.Persistence;
using Microsoft.AspNetCore.Identity;
using Flex.AspNetIdentity.Api.Entities;

namespace Flex.AspNetIdentity.Api.Extensions
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
            services.AddControllers().ApplyJsonSettings();

            services.AddEndpointsApiExplorer();

            services.AddOpenApi();

            services.ConfigureRouteOptions();
            services.ConfigureValidationErrorResponse();

            // Register services DI container
            services.AddInfrastructureServices();

            // Database
            services.ConfigureServiceDbContext<IdentityDbContext>(configuration);

            // Identity
            services.ConfigureAspNetIdentity();

            return services;
        }

        #region Infrastructure
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services;
        }

        private static IServiceCollection ConfigureAspNetIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
        #endregion
    }
}
