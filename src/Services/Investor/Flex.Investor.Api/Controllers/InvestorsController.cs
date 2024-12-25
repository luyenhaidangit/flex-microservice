using Flex.Investor.Api.Services.Interfaces;
using Flex.Shared.SeedWork;

namespace Flex.Investor.Api.Controllers
{
    public static class InvestorsController
    {
        public static void MapInvestorsApi(this WebApplication app)
        {
            app.MapGet("/api/investors",
                async (EntityKey<long> entityKey, IInvestorService customerService) =>
                {
                    var result = await customerService.GetInvestorByIdAsync(entityKey.Id);
                    return result;
                });
        }
    }
}
