using AutoMapper;
using Flex.Infrastructure.EF;
using Flex.Investor.Api.Repositories.Interfaces;
using Flex.Investor.Api.Services.Interfaces;
using Flex.Shared.DTOs.Investor;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Flex.Investor.Api.Services
{
    public class InvestorService : IInvestorService
    {
        private readonly IInvestorRepository _investorRepository;
        private readonly IMapper _mapper;

        public InvestorService(IInvestorRepository investorRepository, IMapper mapper)
        {
            _investorRepository = investorRepository;
            _mapper = mapper;
        }

        #region Query
        public async Task<PagedResult<InvestorDto>> GetPagingInvestorsAsync(GetInvestorsPagingRequest request)
        {
            var query = _investorRepository.FindAll()
                .WhereIf(!string.IsNullOrEmpty(request.Name), b => b.FullName.Contains(request.Name));

            var resultPaged = await query.ToPagedResultAsync(request);
            return resultPaged.MapPagedResult<Flex.Investor.Api.Entities.Investor, InvestorDto>(_mapper);
        }

        public async Task<InvestorDto> GetInvestorByIdAsync(long id)
        {
            var investor = await _investorRepository.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return investor is null ? null : _mapper.Map<InvestorDto>(investor);
        }
        #endregion

        #region Command
        public async Task<Result> CreateInvestorAsync(CreateInvestorRequest request)
        {
            var isEmailExist = await _investorRepository.FindByCondition(x => x.Email == request.Email).AnyAsync();
            if (isEmailExist)
            {
                return Result.Failure("Email is already in use.");
            }

            var investor = _mapper.Map<Flex.Investor.Api.Entities.Investor>(request);
            await _investorRepository.CreateAsync(investor);

            return Result.Success();
        }

        public async Task<Result> UpdateInvestorAsync(UpdateInvestorRequest request)
        {
            var investor = await _investorRepository.FindByCondition(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (investor is null)
            {
                return Result.Failure("Investor not found.");
            }

            _mapper.Map(request, investor);
            await _investorRepository.UpdateAsync(investor);

            return Result.Success();
        }

        public async Task<Result> DeleteInvestorAsync(long id)
        {
            var investor = await _investorRepository.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            if (investor is null)
            {
                return Result.Failure("Investor not found.");
            }

            await _investorRepository.DeleteAsync(investor);
            return Result.Success();
        }
        #endregion
    }
}
