using Flex.Investor.Api.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;

namespace Flex.Investor.Api.Persistence
{
    public static class InvestorDbSeed
    {
        public static async Task<WebApplication> SeedsDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var customerContext = scope.ServiceProvider.GetRequiredService<InvestorDbContext>();

            // Migrate database
            await customerContext.Database.MigrateAsync();

            // Seed datab
            var customers = new List<(string no,string fullName, string email, string phone)>
            {
                ("NO1", "Luyện Hải Đăng", "luyenhaidangit@gmail.com", "0922002360"),
                ("NO2", "Đào Xuân Đức", "daoxuanduc@gmail.com", "0922002361"),
                ("NO3", "Nguyễn Văn A", "luyenhaidangit1@gmail.com", "0922002362"),
            };

            // Seed investors
            foreach (var (no, fullName, email, phone) in customers)
            {
                await InvestorSeed.InitAsync(customerContext, no, fullName, email, phone);
            }

            return app;
        }

    }
}
