using AutoMapper;
using Flex.Shared.DTOs.Customer;

namespace Flex.Customer.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Entities.Customer, CustomerDto>();
        }
    }
}
