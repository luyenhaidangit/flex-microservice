using ILogger = Serilog.ILogger;
using Flex.Securities.Api.Entities;

namespace Flex.Securities.Api.Persistence.Seeds
{
    public static class SecuritiesSeed
    {
        public static async Task InitAsync(SecuritiesDbContext securitiesContext, ILogger logger)
        {
            if (!securitiesContext.Securities.Any())
            {
                var catalogSecurities = new List<CatalogSecurities>()
                {
                };

                securitiesContext.AddRange(catalogSecurities);

                await securitiesContext.SaveChangesAsync();
                logger.Information("Seeded data for Product DB associated with context {DbContextName}", nameof(securitiesContext));
            }
        }
    }
}
