using Flex.Shared.DTOs.Investor;
using Flex.Shared.SeedWork;

namespace Flex.Investor.Api.Services.Interfaces
{
    public interface IInvestorService
    {
        // Investor
        Task<PagedResult<InvestorDto>> GetPagingInvestorsAsync(GetInvestorsPagingRequest request);
        Task<InvestorDto> GetInvestorByIdAsync(long id);
        Task<Result> CreateInvestorAsync(CreateInvestorRequest request);
        Task<Result> UpdateInvestorAsync(UpdateInvestorRequest request);
        Task<Result> DeleteInvestorAsync(long id);

        // SubAccount
        Task<List<SubAccountDto>> GetSubAccountsByInvestorIdAsync(long investorId); 
        Task<Result> CreateSubAccountAsync(CreateSubAccountRequest request);
        Task<Result> UpdateSubAccountAsync(UpdateSubAccountRequest request);
        Task<Result> DeleteSubAccountAsync(long subAccountId);
    }
}
