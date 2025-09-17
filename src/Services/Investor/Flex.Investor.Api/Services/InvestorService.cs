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
            var investorCount = await _investorRepository.FindByCondition(x => x.Id == investorId).CountAsync();
            if (investorCount == 0)
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
            var noCount = await _investorRepository.FindByCondition(x => x.No == request.No).CountAsync();
            if (noCount > 0)
            {
                return Result.Failure(message: "Investor No is already in use.");
            }

            var emailCount = await _investorRepository.FindByCondition(x => x.Email == request.Email).CountAsync();
            if (emailCount > 0)
            {
                return Result.Failure(message: "Email is already in use.");
            }

            var phoneCount = await _investorRepository.FindByCondition(x => x.Phone == request.Phone).CountAsync();
            if (phoneCount > 0)
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
                var emailCount = await _investorRepository.FindByCondition(x => x.Email == request.Email).CountAsync();
                if (emailCount > 0)
                {
                    return Result.Failure(message: "Email is already in use.");
                }
            }

            if (request.Phone.Trim().ToLower() != investor.Phone.Trim().ToLower())
            {
                var phoneCount = await _investorRepository.FindByCondition(x => x.Phone == request.Phone).CountAsync();
                if (phoneCount > 0)
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
            // Validate investor existence
            var investorCount = await _investorRepository.FindByCondition(x => x.Id == request.InvestorId).CountAsync();
            if (investorCount == 0)
            {
                return Result.Failure(message: "Investor not found.");
            }

            // Generate unique AccountNo
            var lastAccount = await _subAccountRepository.FindAll()
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            var nextId = lastAccount != null ? lastAccount.Id + 1 : 1;
            var accountNo = $"{request.AccountType.ToUpper()}-{nextId}";

            var subAccount = new Entities.SubAccount
            {
                InvestorId = request.InvestorId,
                AccountNo = accountNo,
                AccountType = request.AccountType.ToUpper(),
                Balance = 0,
                Status = "ACTIVE"
            };

            await _subAccountRepository.CreateAsync(subAccount);
            return Result.Success();
        }

        public async Task<Result> UpdateSubAccountAsync(UpdateSubAccountRequest request)
        {
            var subAccount = await _subAccountRepository.FindByCondition(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (subAccount is null) return Result.Failure("SubAccount not found.");

            // Ensure the account type is valid before updating
            var validAccountTypes = new List<string> { "CASH", "MARGIN", "DERIVATIVES", "BONDS" };
            if (!validAccountTypes.Contains(request.AccountType.ToUpper()))
            {
                return Result.Failure($"Invalid account type: {request.AccountType}. Allowed types: {string.Join(", ", validAccountTypes)}.");
            }

            // Ensure no duplicate account type exists for the same investor
            var existingAccountCount = await _subAccountRepository.FindByCondition(x => x.InvestorId == subAccount.InvestorId && x.AccountType == request.AccountType && x.Id != request.Id).CountAsync();
            if (existingAccountCount > 0)
            {
                return Result.Failure($"An account of type '{request.AccountType}' already exists for this investor.");
            }

            _mapper.Map(request, subAccount);
            await _subAccountRepository.UpdateAsync(subAccount);
            return Result.Success();
        }
        public async Task<Result> DeleteSubAccountAsync(long subAccountId)
        {
            var subAccount = await _subAccountRepository.FindByCondition(x => x.Id == subAccountId).FirstOrDefaultAsync();
            if (subAccount is null)
            {
                return Result.Failure("SubAccount not found.");
            }

            await _subAccountRepository.DeleteAsync(subAccount);
            return Result.Success();
        }
        #endregion
    }
}
