using Flex.Contracts.Domains;
using Flex.Shared.Constants;
using Flex.Shared.Enums.General;

namespace Flex.Securities.Api.Entities
{
    public class CatalogIssuerRequest : EntityRequestAuditBase<long>
    {
        public ERequestType Type { get; set; }

        public ERequestStatus Status { get; set; }

        public string DataProposed { get; set; } = Json.EMPTY_JSON;
    }
}
