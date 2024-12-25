using AutoMapper;
using Flex.Investor.Api.Repositories.Interfaces;
using Flex.Investor.Api.Services.Interfaces;
using Flex.Shared.DTOs.Investor;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
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
        /// <summary>
        /// Thông tin Chi tiết nhà đầu tư.
        /// </summary>
        public async Task<IResult> GetInvestorByIdAsync([FromQuery] EntityKey<long> entityKey)
        {
            var securities = await _investorRepository.GetSampleData().Where(x => x.Id == entityKey.Id).FirstOrDefaultAsync();
            if (securities is null)
            {
                return Results.BadRequest(Result.Failure(message: "Securities not found."));
            }

            var result = _mapper.Map<InvestorDto>(securities);

            return Results.Ok(Result.Success(result));
        }
        #endregion
    }
}
