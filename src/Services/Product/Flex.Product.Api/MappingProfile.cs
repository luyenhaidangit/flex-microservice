using AutoMapper;
using Flex.Product.Api.Entities;
using Flex.Shared.DTOs.Products;
using Flex.Infrastructure.Mappings;

namespace Flex.Product.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CatalogProduct, ProductDto>();
            CreateMap<CreateProductDto, CatalogProduct>();
            CreateMap<UpdateProductDto, CatalogProduct>()
            .IgnoreAllNonExisting();
        }
    }
}
