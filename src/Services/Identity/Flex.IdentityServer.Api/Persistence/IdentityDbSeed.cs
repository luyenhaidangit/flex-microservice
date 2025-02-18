using ILogger = Serilog.ILogger;
using Flex.IdentityServer.Api.Persistence.Seeds;

namespace Flex.IdentityServer.Api.Persistence
{
    public static class IdentityDbSeed
    {
        public static async Task InitAsync(IServiceProvider services, ILogger logger)
        {
            var scope = services.CreateScope();
            await IdentitySeed.InitAspnetIdentityAsync(scope.ServiceProvider, logger);
            await IdentitySeed.InitIdentityServerAsync(scope.ServiceProvider);
        }
    }
}
