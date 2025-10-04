using AutoMapper;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Models.Requests;
using System.Text.Json;

namespace Flex.Workflow.Api.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WorkflowRequest, PendingRequestListItemDto>()
                .ForMember(d => d.RequestId, o => o.MapFrom(s => s.Id));

            CreateMap<WorkflowAction, RequestActionDto>();
            CreateMap<WorkflowAuditLog, RequestAuditDto>();

            CreateMap<WorkflowRequest, RequestDetailDto>()
                .ForMember(d => d.RequestId, o => o.MapFrom(s => s.Id));
        }
    }
}
