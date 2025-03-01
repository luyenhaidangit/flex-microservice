using AutoMapper;
using Flex.Infrastructure.Mappings;
using Flex.Investor.Api.Entities;
using Flex.Shared.DTOs.Investor;

namespace Flex.Investor.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Investor mappings
            CreateMap<CreateInvestorRequest, Entities.Investor>();
            CreateMap<UpdateInvestorRequest, Entities.Investor>().IgnoreAllNonExisting();
            CreateMap<Entities.Investor, InvestorDto>();

            // SubAccount mappings
            CreateMap<CreateSubAccountRequest, SubAccount>();
            CreateMap<UpdateSubAccountRequest, SubAccount>().IgnoreAllNonExisting();
            CreateMap<SubAccount, SubAccountDto>();
        }
    }
}
