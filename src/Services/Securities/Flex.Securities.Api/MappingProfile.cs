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
            CreateMap<CatalogIssuer, IssuerPagedDto>();
            CreateMap<CatalogIssuer, IssuerDto>();
            CreateMap<CreateIssuerDto, CatalogIssuer>();
            CreateMap<UpdateIssuerDto, CatalogIssuer>();

            // Security
            CreateMap<CatalogSecurities, SecurityDto>();
            CreateMap<CreateSecuritiesDto, CatalogSecurities>();
            CreateMap<UpdateSecuritiesDto, CatalogSecurities>().IgnoreAllNonExisting();
        }
    }
}
