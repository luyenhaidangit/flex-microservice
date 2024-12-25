using Flex.Shared.SeedWork;

namespace Flex.Investor.Api.Services.Interfaces
{
    public interface IInvestorService
    {
        Task<IResult> GetInvestorByIdAsync(long id);
    }
}
