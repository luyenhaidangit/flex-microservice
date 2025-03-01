using AutoMapper;
using Flex.Infrastructure.EF;
using Flex.Investor.Api.Repositories.Interfaces;
using Flex.Investor.Api.Services.Interfaces;
using Flex.Shared.DTOs.Investor;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;
using Flex.Infrastructure.Exceptions;
using Flex.Investor.Api.Repositories;

namespace Flex.Investor.Api.Services
{
    public class InvestorService : IInvestorService
    {
        private readonly IInvestorRepository _investorRepository;
        private readonly ISubAccountRepository _subAccountRepository;
        private readonly IMapper _mapper;

        public InvestorService(IMapper mapper, IInvestorRepository investorRepository, ISubAccountRepository subAccountRepository)
        {
            _mapper = mapper;
            _subAccountRepository = subAccountRepository;
            _investorRepository = investorRepository;
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

        public async Task<List<SubAccountDto>> GetSubAccountsByInvestorIdAsync(long investorId)
        {
            // Validate investor existence
            var investorExists = await _investorRepository.FindByCondition(x => x.Id == investorId).AnyAsync();
            if (!investorExists)
            {
                throw new BadRequestException("Investor not found.");
            }

            var subAccounts = await _subAccountRepository.FindByCondition(x => x.InvestorId == investorId).ToListAsync();

            return _mapper.Map<List<SubAccountDto>>(subAccounts);
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
            // TRANSACTION BEGIN
            await using var transaction = await _investorRepository.BeginTransactionAsync();

            var investor = _mapper.Map<Entities.Investor>(request);
            await _investorRepository.CreateAsync(investor);

            var lastAccount = await _subAccountRepository.FindAll()
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();

            var nextId = lastAccount != null ? lastAccount.Id + 1 : 1;
            var accountNo = $"CASH-{nextId}";

            // Defaut sub account: CASH
            var defaultSubAccount = new Entities.SubAccount
            {
                InvestorId = investor.Id,
                AccountNo = accountNo,
                AccountType = "CASH",
                Balance = 0,
                Status = "ACTIVE"
            };
            await _subAccountRepository.CreateAsync(defaultSubAccount);

            await transaction.CommitAsync();
            // TRANSACTION END

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
                    return Result.Failure("Phone is already in use.");
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
                return Result.Failure("Investor not found.");
            }

            // Process
            await _investorRepository.DeleteAsync(investor);
            return Result.Success();
        }

        public async Task<Result> CreateSubAccountAsync(CreateSubAccountRequest request)
        {
            var subAccount = _mapper.Map<Entities.SubAccount>(request);
            await _subAccountRepository.CreateAsync(subAccount);
            return Result.Success();
        }

        public async Task<Result> UpdateSubAccountAsync(UpdateSubAccountRequest request)
        {
            var subAccount = await _subAccountRepository.FindByCondition(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (subAccount is null) return Result.Failure("SubAccount not found.");

            _mapper.Map(request, subAccount);
            await _subAccountRepository.UpdateAsync(subAccount);
            return Result.Success();
        }

        public async Task<Result> DeleteSubAccountAsync(long subAccountId)
        {
            var subAccount = await _subAccountRepository.FindByCondition(x => x.Id == subAccountId).FirstOrDefaultAsync();
            if (subAccount is null) return Result.Failure("SubAccount not found.");

            await _subAccountRepository.DeleteAsync(subAccount);
            return Result.Success();
        }
        #endregion
    }
}
