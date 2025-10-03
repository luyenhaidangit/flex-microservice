using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Extensions
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

        public static async Task<IHost> MigrateDatabase<TContext>(this IHost host, Func<TContext, IServiceProvider, Task> seeder) where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILogger<TContext>>();

            var context = services.GetRequiredService<TContext>();

            try
            {
                // Migrate database
                logger.LogInformation("Starting database migration for {DbContextName}...", typeof(TContext).Name);

                await context.Database.MigrateAsync();

                await seeder(context, services);

                logger.LogInformation("Migrated {DbContextName} database successfully", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the {DbContextName} database", typeof(TContext).Name);
                throw;
            }

            return host;
        }
    }
}
