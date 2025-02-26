using Flex.Shared.DTOs.Investor;
using Flex.Shared.SeedWork;

namespace Flex.Investor.Api.Services.Interfaces
{
    public interface IInvestorService
    {
        Task<PagedResult<InvestorDto>> GetPagingInvestorsAsync(GetInvestorsPagingRequest request);
        Task<InvestorDto> GetInvestorByIdAsync(long id);
        Task<Result> CreateInvestorAsync(CreateInvestorRequest request);
        Task<Result> UpdateInvestorAsync(UpdateInvestorRequest request);
        Task<Result> DeleteInvestorAsync(long id);
    }
}
