using AutoMapper;
using Flex.AspNetIdentity.Api.Entities;
using Flex.Shared.DTOs.Identity;

namespace Flex.AspNetIdentity.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Identity
            CreateMap<RegisterUserRequest, User>();
            CreateMap<Models.CreateUserDto, User>();
        }
    }
}
