using AutoMapper;
using Flex.Infrastructure.Mappings;
using Flex.Shared.DTOs.System;
using Flex.Shared.DTOs.System.Department;
using Flex.System.Api.Entities;

namespace Flex.System.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Config, ConfigDto>();
            CreateMap<CreateConfigRequest, Config>();
            CreateMap<UpdateConfigRequest, Config>().IgnoreAllNonExisting();

            // Department mappings
            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentRequest, DepartmentRequest>();
            CreateMap<UpdateDepartmentRequest, Department>().IgnoreAllNonExisting();
        }
    }
}
