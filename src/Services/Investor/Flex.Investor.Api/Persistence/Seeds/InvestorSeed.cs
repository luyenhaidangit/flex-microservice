using Microsoft.EntityFrameworkCore;

namespace Flex.Investor.Api.Persistence.Seeds
{
    public static class InvestorSeed
    {
        public static async Task InitAsync(InvestorDbContext investorContext, string no, string fullname, string email, string phone)
        {
            if (!await investorContext.Investors.AnyAsync(x => x.No == no || x.Email == email || x.Phone == phone))
            {
                var newInvestor = new Entities.Investor
                {
                    No = no,
                    FullName = fullname,
                    Email = email,
                    Phone = phone,
                };

                await investorContext.Investors.AddAsync(newInvestor);
                await investorContext.SaveChangesAsync();
            }
        }
    }
}
