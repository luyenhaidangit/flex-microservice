using AutoMapper;
using Flex.Shared.DTOs.Investor;

namespace Flex.Investor.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateInvestorRequest, Entities.Investor>();
            CreateMap<UpdateInvestorRequest, Entities.Investor>();
            CreateMap<Entities.Investor, InvestorDto>();
        }
    }
}
