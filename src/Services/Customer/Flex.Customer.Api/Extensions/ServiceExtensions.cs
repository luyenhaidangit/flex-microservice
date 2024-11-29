using Flex.Customer.Api.Persistence;
using Flex.Customer.Api.Repositories;
using Flex.Customer.Api.Repositories.Interfaces;
using Flex.Customer.Api.Services;
using Flex.Customer.Api.Services.Interfaces;
using Flex.Shared.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Flex.Customer.Api.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
        {
            var databaseSettings = configuration.GetSection(nameof(DatabaseSettings))
                .Get<DatabaseSettings>();
            services.AddSingleton(databaseSettings);

            return services;
        }

        public static void ConfigureCustomerContext(this IServiceCollection services)
        {
            var databaseSettings = services.BuildServiceProvider().GetService<DatabaseSettings>();
            if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
                throw new ArgumentNullException("Connection string is not configured.");

            services.AddDbContext<CustomerContext>(
                options => options.UseNpgsql(databaseSettings.ConnectionString));
        }

        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<ICustomerService, CustomerService>();
        }

        //public static void ConfigureHealthChecks(this IServiceCollection services)
        //{
        //    var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        //    services.AddHealthChecks()
        //        .AddNpgSql(databaseSettings.ConnectionString,
        //            name: "PostgresQL Health",
        //            failureStatus: HealthStatus.Degraded);
        //}
    }
}
