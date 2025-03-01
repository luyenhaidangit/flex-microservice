using AutoMapper;
using Flex.Infrastructure.EF;
using Flex.Investor.Api.Repositories.Interfaces;
using Flex.Investor.Api.Services.Interfaces;
using Flex.Shared.DTOs.Investor;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;
using Flex.Infrastructure.Exceptions;

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
                            .WhereIf(!string.IsNullOrEmpty(request.No), b => b.No.ToLower().Contains(request.No.ToLower()))
                            .WhereIf(!string.IsNullOrEmpty(request.Name), b => b.FullName.ToLower().Contains(request.Name.ToLower()));

            var resultPaged = await query.ToPagedResultAsync(request);
            return resultPaged.MapPagedResult<Entities.Investor, InvestorDto>(_mapper);
        }

        public async Task<InvestorDto> GetInvestorByIdAsync(long id)
        {
            var investor = await _investorRepository.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            if (investor is null)
            {
                throw new BadRequestException("Investor not found.");
            }

            var result = _mapper.Map<InvestorDto>(investor);

            return result;
        }
        #endregion

        #region Command
        public async Task<Result> CreateInvestorAsync(CreateInvestorRequest request)
        {
            // Validate
            var isNoExist = await _investorRepository.FindByCondition(x => x.No == request.No).AnyAsync();
            if (isNoExist)
            {
                return Result.Failure(message: "Investor No is already in use.");
            }

            var isEmailExist = await _investorRepository.FindByCondition(x => x.Email == request.Email).AnyAsync();
            if (isEmailExist)
            {
                return Result.Failure(message: "Email is already in use.");
            }

            var isPhoneExist = await _investorRepository.FindByCondition(x => x.Phone == request.Phone).AnyAsync();
            if (isPhoneExist)
            {
                return Result.Failure(message: "Phone number is already in use.");
            }

            // Process
            var investor = _mapper.Map<Entities.Investor>(request);
            await _investorRepository.CreateAsync(investor);

            return Result.Success();
        }

        public async Task<Result> UpdateInvestorAsync(UpdateInvestorRequest request)
        {
            // Validate
            var investor = await _investorRepository.FindByCondition(x => x.No == request.No).FirstOrDefaultAsync();
            if (investor is null)
            {
                return Result.Failure(message: "Investor not found.");
            }

            if (request.Email.Trim().ToLower() != investor.Email.Trim().ToLower())
            {
                var isNoExist = await _investorRepository.FindByCondition(x => x.No == request.No).AnyAsync();
                if (isNoExist)
                {
                    return Result.Failure(message: "Email No is already in use.");
                }
            }

            if (request.Phone.Trim().ToLower() != investor.Phone.Trim().ToLower())
            {
                var isEmailExist = await _investorRepository.FindByCondition(x => x.Email == request.Email).AnyAsync();
                if (isEmailExist)
                {
                    return Result.Failure(message: "Phone is already in use.");
                }
            }
            
            // Process
            _mapper.Map(request, investor);
            await _investorRepository.UpdateAsync(investor);

            return Result.Success();
        }

        public async Task<Result> DeleteInvestorAsync(long id)
        {
            // Validate
            var investor = await _investorRepository.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            if (investor is null)
            {
                return Result.Failure(message: "Investor not found.");
            }

            // Process
            await _investorRepository.DeleteAsync(investor);
            return Result.Success();
        }
        #endregion
    }
}
