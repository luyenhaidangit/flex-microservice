using Flex.Shared.Attributes;
using Flex.Shared.Enums;
using Flex.Shared.SeedWork;

namespace Flex.Shared.DTOs.Securities
{
    public class GetIssuersPagingRequest : PagingRequest
    {
        public string? Name { get; set; }

        public EEntityStatus? Status { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
            { "NAME", "IssuerName" },
            { "CREATEDDATE", "CreatedDate" },
            { "UPDATEDDATE", "UpdatedDate" }
        };
    }
}
