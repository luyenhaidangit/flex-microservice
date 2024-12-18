using Flex.Contracts.Domains;
using Flex.Shared.Constants;
using Flex.Shared.Enums.General;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Elasticsearch.Net.Specification.SecurityApi;
using Flex.Shared.SeedWork;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Flex.Securities.Api.Entities
{
    [Table("ISSUERREQUESTS")]
    public class CatalogIssuerRequest : EntityRequestAuditBase<long, long?>
    {
        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public ERequestType Type { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public ERequestStatus Status { get; set; }

        [Column(TypeName = "CLOB")]
        public string DataProposed { get; set; } = Json.EMPTY_JSON;

        public CatalogIssuerRequest(string dataProposed, ERequestType type, ERequestStatus status)
        {
            this.DataProposed = dataProposed;
            this.Type = type;
            this.Status = status;
        }

        public static CatalogIssuerRequest Create(string dataProposed, ERequestType type, ERequestStatus status)
        {
            return new CatalogIssuerRequest(dataProposed, type, status);
        }
    }
}
