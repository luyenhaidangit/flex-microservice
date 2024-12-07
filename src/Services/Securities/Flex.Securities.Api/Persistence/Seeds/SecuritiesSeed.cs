using ILogger = Serilog.ILogger;
using Flex.Securities.Api.Entities;
using Flex.Shared.Enums.Securities;

namespace Flex.Securities.Api.Persistence.Seeds
{
    public static class SecuritiesSeed
    {
        public static async Task InitAsync(SecuritiesDbContext securitiesContext, ILogger logger)
        {
            if (!securitiesContext.CatalogSecurities.Any())
            {
                var catalogSecurities = new List<CatalogSecurities>()
                {
                    new CatalogSecurities
                    {
                        No = "000001",
                        Symbol = "AAA",
                        IssuerId = "0000000001",
                        TradePlace = ETradePlace.Hose,
                        Description = ""
                    }
                };

                securitiesContext.AddRange(catalogSecurities);

                await securitiesContext.SaveChangesAsync();
                logger.Information("Seeded data for Product DB associated with context {DbContextName}", nameof(securitiesContext));
            }
        }
    }
}
