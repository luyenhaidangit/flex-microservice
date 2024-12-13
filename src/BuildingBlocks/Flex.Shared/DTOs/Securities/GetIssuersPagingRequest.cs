using Flex.Shared.Enums;
using Flex.Shared.SeedWork;

namespace Flex.Shared.DTOs.Securities
{
    public class GetIssuersPagingRequest : PagingRequestParameters
    {
        public string Name { get; set; }

        public EEntityStatus Status { get; set; }
    }
}
