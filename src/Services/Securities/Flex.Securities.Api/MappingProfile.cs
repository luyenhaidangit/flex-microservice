using AutoMapper;
using Flex.Infrastructure.Mappings;
using Flex.Securities.Api.Entities;
using Flex.Shared.DTOs.Securities;

namespace Flex.Securities.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CatalogSecurity, SecuritiesDto>();
            CreateMap<CreateSecuritiesDto, CatalogSecurity>();
            CreateMap<UpdateSecuritiesDto, CatalogSecurity>().IgnoreAllNonExisting();
        }
    }
}
