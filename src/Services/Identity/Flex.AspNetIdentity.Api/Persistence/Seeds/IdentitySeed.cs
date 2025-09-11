using Flex.AspNetIdentity.Api.Entities;
using Flex.Shared.SeedWork.Workflow.Constants;
using Microsoft.AspNetCore.Identity;
using ILogger = Serilog.ILogger;

namespace Flex.AspNetIdentity.Api.Persistence.Seeds
{
    public static class IdentitySeed
    {
        public static async Task InitAsync(IServiceProvider servicesProvider, ILogger logger)
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
                        await roleManager.CreateAsync(new Role(role,"0000"));
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
                        EmailConfirmed = true,
                        Status = RequestStatusConstant.Authorised,
                    };

                    var result = await userManager.CreateAsync(adminUser, "Haidang106@");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.Information("Seeded Admin User: {AdminEmail}", adminEmail);
                    }
                    else
                    {
                        logger.Error("Failed to seed Admin User. Errors: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.Information("Assigned Admin role to user: {AdminEmail}", adminEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error seeding Identity Database: {ExceptionMessage}", ex.Message);
            }
        }
    }
}
