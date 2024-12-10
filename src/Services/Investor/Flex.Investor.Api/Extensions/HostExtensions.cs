using Microsoft.EntityFrameworkCore;

namespace Flex.Investor.Api.Extensions
{
    public static class HostExtensions
    {
        public static void AddAppConfigurations(this WebApplicationBuilder builder)
        {
            var env = builder.Environment;

            //  Adds application configurations from JSON files and environment variables.
            builder.Configuration
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();
        }

        public static IHost MigrateDatabase<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating mysql database.");
                    ExecuteMigrations(context);
                    logger.LogInformation("Migrated mysql database.");
                    InvokeSeeder(seeder, context, services);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the mysql database");
                }
            }

            return host;
        }

        private static void ExecuteMigrations<TContext>(TContext context)
        where TContext : DbContext
        {
            //context.Database.Migrate();
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
            IServiceProvider services) where TContext : DbContext
        {
            seeder(context, services);
        }
    }
}
