using AutoMapper;
using Flex.Infrastructure.Mappings;
using Flex.Securities.Api.Entities;
using Flex.Shared.DTOs.Securities;
using Flex.Shared.Extensions;

namespace Flex.Securities.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Security
            CreateMap<CatalogSecurities, SecuritiesDto>();
            CreateMap<CreateSecuritiesRequest, CatalogSecurities>();
            CreateMap<UpdateSecuritiesRequest, CatalogSecurities>().IgnoreAllNonExisting();
        }
    }
}
