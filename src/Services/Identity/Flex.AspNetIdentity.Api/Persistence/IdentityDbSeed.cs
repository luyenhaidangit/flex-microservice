using Flex.AspNetIdentity.Api.Persistence.Seeds;
using ILogger = Serilog.ILogger;

namespace Flex.AspNetIdentity.Api.Persistence
{
    public static class IdentityDbSeed
    {
        public static async Task InitAsync(IServiceProvider services, ILogger logger)
        {
            var scope = services.CreateScope();
            await IdentitySeed.InitAsync(scope.ServiceProvider, logger);
        }
    }
}
