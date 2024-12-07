using ILogger = Serilog.ILogger;
using Flex.Securities.Api.Persistence.Seeds;

namespace Flex.Securities.Api.Persistence
{
    public class SecuritiesDbSeed
    {
        public static async Task InitAsync(SecuritiesDbContext securitiesContext, ILogger logger)
        {
            await SecuritiesSeed.InitAsync(securitiesContext, logger);
        }
    }
}
