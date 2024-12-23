using AutoMapper;
using Flex.Infrastructure.EF;
using Flex.Investor.Api.Repositories.Interfaces;
using Flex.Investor.Api.Services.Interfaces;
using Flex.Shared.DTOs.Investor;
using Flex.Shared.SeedWork;

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

        public Task<Result> CreateInvestorAsync(CreateInvestorRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteInvestorAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<InvestorDto> GetInvestorByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> GetPagingInvestorsAsync(GetInvestorsPagingRequest request)
        {
            var query = _investorRepository.FindAll()
                .WhereIf(!string.IsNullOrEmpty(request.No), b => b.No.ToUpper().Contains(request.No.ToUpper()))
                .WhereIf(!string.IsNullOrEmpty(request.Name), b => b.FullName.ToUpper().Contains(request.Name.ToUpper()));

            var resultPaged = await query.ToPagedResultAsync(request);

            var resultDtoPaged = resultPaged.MapPagedResult<Entities.Investor, InvestorDto>(_mapper);

            return Results.Ok(Result.Success(resultDtoPaged));
        }

        public Task<Result> UpdateInvestorAsync(UpdateInvestorRequest request)
        {
            throw new NotImplementedException();
        }

        Task<PagedResult<InvestorDto>> IInvestorService.GetPagingInvestorsAsync(GetInvestorsPagingRequest request)
        {
            throw new NotImplementedException();
        }

        //public async Task<InvestorDto> GetInvestorByIdAsync(long id)
        //{
        //    var entity = await _repository.GetInvestorByIdAsync(id);
        //    return _mapper.Map<InvestorDto>(entity);
        //}

        //public async Task<Result> CreateInvestorAsync(CreateInvestorRequest request)
        //{
        //    var entity = _mapper.Map<Investor>(request);
        //    var result = await _repository.CreateInvestorAsync(entity);
        //    return new Result { Success = result, Message = result ? "Investor created successfully" : "Failed to create investor" };
        //}

        //public async Task<Result> UpdateInvestorAsync(UpdateInvestorRequest request)
        //{
        //    var entity = _mapper.Map<Investor>(request);
        //    var result = await _repository.UpdateInvestorAsync(entity);
        //    return new Result { Success = result, Message = result ? "Investor updated successfully" : "Failed to update investor" };
        //}

        //public async Task<Result> DeleteInvestorAsync(long id)
        //{
        //    var result = await _repository.DeleteInvestorAsync(id);
        //    return new Result { Success = result, Message = result ? "Investor deleted successfully" : "Failed to delete investor" };
        //}
    }
}
