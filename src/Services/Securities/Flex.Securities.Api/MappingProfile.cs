using AutoMapper;
using Flex.Securities.Api.Entities;
using Flex.Shared.DTOs.Securities;

namespace Flex.Securities.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CatalogSecurities, SecuritiesDto>();
            CreateMap<CreateSecuritiesDto, CatalogSecurities>();
        }
    }
}
