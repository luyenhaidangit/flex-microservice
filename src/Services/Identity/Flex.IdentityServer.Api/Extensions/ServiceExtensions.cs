using Flex.Infrastructure.Swashbuckle;
using Flex.Shared.Extensions;
using Flex.IdentityServer.Api.Persistence;
using Flex.EntityFrameworkCore.Oracle;
using Microsoft.EntityFrameworkCore;
using Flex.IdentityServer.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Flex.Security;

namespace Flex.IdentityServer.Api.Extensions
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

            services.ConfigureSwagger();

            services.ConfigureRouteOptions();
            services.ConfigureValidationErrorResponse();

            // Register services DI container
            services.AddInfrastructureServices();

            // Identity Server
            services.ConfigureAspNetIdentity();
            services.AddJwtTokenSecurity(configuration);
            services.ConfigureIdentityServer(configuration);

            // Database
            services.ConfigureServiceDbContext<IdentityDbContext>(configuration);

            // AutoMapper
            services.AddAutoMapper(AssemblyReference.Assembly);

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

        private static IServiceCollection ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddAspNetIdentity<User>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = dbContextBuilder =>
                {
                    dbContextBuilder.UseOracle(
                        configuration.GetConnectionString("DefaultConnection"),
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(AssemblyReference.Assembly);
                        });
                };
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = dbContextBuilder =>
                {
                    dbContextBuilder.UseOracle(
                        configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(AssemblyReference.Assembly);
                    });
                };

                // Delete expired tokens 1 hours
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 3600;
            })
            .AddDeveloperSigningCredential();

            return services;
        }
        #endregion
    }
}
