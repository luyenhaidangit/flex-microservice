using Microsoft.AspNetCore.Identity;
using Flex.IdentityServer.Api.Entities;
using ILogger = Serilog.ILogger;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Flex.IdentityServer.Api.Configurations;

using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Flex.IdentityServer.Api.Persistence.Seeds
{
    public static class IdentitySeed
    {
        public static async Task InitAspnetIdentityAsync(IServiceProvider servicesProvider, ILogger logger)
        {
            var userManager = servicesProvider.GetRequiredService<UserManager<User>>();
            var roleManager = servicesProvider.GetRequiredService<RoleManager<Role>>();

            try
            {
                // 1. Default Roles
                var roles = new List<string> { "Admin" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new Role(role));
                        logger.Information("Seeded role {RoleName}", role);
                    }
                }

                // 2. Default Admin User
                var adminEmail = "luyenhaidangit@gmail.com";
                var adminUsername = "admin";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = adminUsername,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin@123");

                    if (result.Succeeded)
                    {
                        //await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.Information("Seeded Admin User: {AdminEmail}", adminEmail);
                    }
                    else
                    {
                        logger.Error("Failed to seed Admin User. Errors: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error seeding Identity Database: {ExceptionMessage}", ex.Message);
            }
        }

        public static async Task InitIdentityServerAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var configDb = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            await configDb.Database.MigrateAsync();

            var persistedGrantDb = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
            await persistedGrantDb.Database.MigrateAsync();

            // Seed Clients
            if (!configDb.Clients.Any())
            {
                foreach (var client in Clients.GetClients())
                {
                    configDb.Clients.Add(client.ToEntity());
                }
                configDb.SaveChanges();
            }

            // Seed ApiScopes
            if (!configDb.ApiScopes.Any())
            {
                foreach (var scopeItem in ApiScopes.GetApiScopes())
                {
                    configDb.ApiScopes.Add(scopeItem.ToEntity());
                }
                configDb.SaveChanges();
            }

            if (!configDb.ApiResources.Any())
            {
                foreach (var apiResource in ApiResources.GetApiResources())
                {
                    configDb.ApiResources.Add(apiResource.ToEntity());
                }
                await configDb.SaveChangesAsync();
            }

            // Seed IdentityResources
            if (!configDb.IdentityResources.Any())
            {
                foreach (var identityResource in IdentityResources.GetIdentityResources())
                {
                    configDb.IdentityResources.Add(identityResource.ToEntity());
                }
                await configDb.SaveChangesAsync();
            }
        }
    }
}
