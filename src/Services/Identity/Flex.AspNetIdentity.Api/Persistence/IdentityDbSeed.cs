using ILogger = Serilog.ILogger;

namespace Flex.AspNetIdentity.Api.Persistence
{
    public static class IdentityDbSeed
    {
        public static async Task InitAsync(IdentityDbContext identityDbContext, ILogger logger)
        {
            await Task.CompletedTask;
        }
    }
}
