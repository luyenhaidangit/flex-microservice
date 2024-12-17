using AutoMapper;
using Flex.Infrastructure.Mappings;
using Flex.Securities.Api.Entities;
using Flex.Shared.DTOs.Securities;
using Flex.Shared.Enums.General;

namespace Flex.Securities.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Issuer
            CreateMap<CatalogIssuer, IssuerPagedDto>()
            .ForMember(dest => dest.Status,
                       opt => opt.MapFrom(src => MapStatusEnumToString(src.Status)));

            CreateMap<CatalogIssuer, IssuerDto>();
            CreateMap<CreateIssuerDto, CatalogIssuer>();
            CreateMap<UpdateIssuerDto, CatalogIssuer>();

            // Security
            CreateMap<CatalogSecurities, SecurityDto>();
            CreateMap<CreateSecuritiesDto, CatalogSecurities>();
            CreateMap<UpdateSecuritiesDto, CatalogSecurities>().IgnoreAllNonExisting();
        }

        private static string MapStatusEnumToString(EEntityStatus status)
        {
            return status switch
            {
                EEntityStatus.PENDING => "P",
                EEntityStatus.ACTIVE => "A",
                EEntityStatus.INACTIVE => "I",
                EEntityStatus.DELETED => "D",
                _ => throw new ArgumentException("Invalid Status Enum")
            };
        }
    }
}
