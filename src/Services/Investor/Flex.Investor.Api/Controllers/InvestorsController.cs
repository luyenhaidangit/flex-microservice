using Flex.Investor.Api.Services.Interfaces;

namespace Flex.Investor.Api.Controllers
{
    public static class InvestorsController
    {
        public static void MapInvestorsApi(this WebApplication app)
        {
            app.MapGet("/api/investors/get-investor-by-id",
                async (long id, IInvestorService customerService) =>
                {
                    var result = await customerService.GetInvestorByIdAsync(id);
                    return result;
                });
        }
    }
}
