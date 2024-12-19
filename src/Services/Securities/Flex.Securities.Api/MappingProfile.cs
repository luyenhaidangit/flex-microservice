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
            // Issuer
            CreateMap<CreateIssuerDto, CatalogIssuerRequest>();
            CreateMap<CatalogIssuerRequest, CatalogIssuer>().ForMember(dest => dest.Id, opt => opt.Ignore());
            //CreateMap<CatalogIssuer, IssuerPagedDto>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToValue()));
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
