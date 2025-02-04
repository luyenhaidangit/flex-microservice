using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;

namespace Flex.EntityFrameworkCore.Oracle
{
    public static class EntityFrameworkCoreExtensions
    {
        public static IServiceCollection ConfigureServiceDbContext<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            OracleConfiguration.SqlNetAllowedLogonVersionClient = OracleAllowedLogonVersionClient.Version11;

            services.AddDbContext<TContext>(options =>
                options.UseOracle(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
