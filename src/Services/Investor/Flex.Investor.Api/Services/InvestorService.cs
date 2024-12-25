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
        public async Task<IResult> GetInvestorByIdAsync(long id)
        {
            var securities = _investorRepository.GetSampleData().Where(x => x.Id == id).FirstOrDefault();
            if (securities is null)
            {
                return Results.BadRequest(Result.Failure(message: "Investor not found."));
            }

            var result = _mapper.Map<InvestorDto>(securities);

            return await Task.FromResult(Results.Ok(Result.Success(result)));
        }
        #endregion
    }
}
