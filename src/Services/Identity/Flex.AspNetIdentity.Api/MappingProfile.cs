using AutoMapper;
using Flex.AspNetIdentity.Api.Entities;
using Flex.Shared.DTOs.Identity;
using Flex.Shared.DTOs.Securities;

namespace Flex.AspNetIdentity.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Identity
            CreateMap<RegisterUserRequest, User>();
        }
    }
}
