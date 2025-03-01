using AutoMapper;
using Flex.Infrastructure.Mappings;
using Flex.Shared.DTOs.Investor;

namespace Flex.Investor.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateInvestorRequest, Entities.Investor>();
            CreateMap<UpdateInvestorRequest, Entities.Investor>().IgnoreAllNonExisting();
            CreateMap<Entities.Investor, InvestorDto>();
        }
    }
}
