using Flex.Infrastructure.Swashbuckle;
using Flex.Shared.Extensions;
using Flex.Identity.Api.Persistence;
using Flex.EntityFrameworkCore.Oracle;
using Flex.Identity.Api.Configurations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace Flex.Identity.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection ConfigureServiceDbContext<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            OracleConfiguration.SqlNetAllowedLogonVersionClient = OracleAllowedLogonVersionClient.Version11;

            services.AddDbContext<TContext>(options =>
                options.UseOracle(configuration.GetConnectionString("DefaultConnection")));

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

        private static IServiceCollection ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
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
            });

            return services;
        }
        #endregion
    }
}
