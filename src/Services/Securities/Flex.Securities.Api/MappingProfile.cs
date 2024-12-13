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
            // Issuer
            CreateMap<CatalogIssuer, IssuerDto>();
            CreateMap<CreateIssuerDto, CatalogIssuer>();
            CreateMap<UpdateIssuerDto, CatalogIssuer>();

            // Security
            CreateMap<CatalogSecurity, SecurityDto>();
            CreateMap<CreateSecuritiesDto, CatalogSecurity>();
            CreateMap<UpdateSecurityDto, CatalogSecurity>().IgnoreAllNonExisting();
        }
    }
}
