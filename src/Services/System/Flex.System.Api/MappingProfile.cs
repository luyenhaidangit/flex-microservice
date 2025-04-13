using AutoMapper;
using Flex.Infrastructure.Mappings;
using Flex.Shared.DTOs.System;
using Flex.Shared.DTOs.System.Branch;
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

            // Branch
            CreateMap<Branch, BranchDto>().ReverseMap();
            CreateMap<CreateBranchRequest, Branch>();
            CreateMap<UpdateBranchRequest, Branch>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
