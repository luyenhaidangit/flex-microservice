using ILogger = Serilog.ILogger;
using Flex.IdentityServer.Api.Persistence.Seeds;

namespace Flex.IdentityServer.Api.Persistence
{
    public static class IdentityDbSeed
    {
        public static async Task InitAsync(IServiceProvider services, ILogger logger)
        {
            var scope = services.CreateScope();
            //await IdentitySeed.InitAsync(scope.ServiceProvider, logger);
        }
    }

    //public static class IdentityDbSeed
    //{
    //    public static async Task InitAsync(IServiceProvider serviceProvider)
    //    {
    //        using var scope = serviceProvider.CreateScope();

    //        var configDb = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    //        await configDb.Database.MigrateAsync();

    //        var persistedGrantDb = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
    //        await persistedGrantDb.Database.MigrateAsync();

    //        // Seed Clients
    //        if (!configDb.Clients.Any())
    //        {
    //            foreach (var client in Clients.GetClients())
    //            {
    //                configDb.Clients.Add(client.ToEntity());
    //            }
    //            configDb.SaveChanges();
    //        }

    //        // Seed ApiScopes
    //        if (!configDb.ApiScopes.Any())
    //        {
    //            foreach (var scopeItem in ApiScopes.GetApiScopes())
    //            {
    //                configDb.ApiScopes.Add(scopeItem.ToEntity());
    //            }
    //            configDb.SaveChanges();
    //        }

    //        if (!configDb.ApiResources.Any())
    //        {
    //            foreach (var apiResource in ApiResources.GetApiResources())
    //            {
    //                configDb.ApiResources.Add(apiResource.ToEntity());
    //            }
    //            await configDb.SaveChangesAsync();
    //        }

    //        // Seed IdentityResources
    //        if (!configDb.IdentityResources.Any())
    //        {
    //            foreach (var identityResource in IdentityResources.GetIdentityResources())
    //            {
    //                configDb.IdentityResources.Add(identityResource.ToEntity());
    //            }
    //            await configDb.SaveChangesAsync();
    //        }
    //    }
    //}
}
