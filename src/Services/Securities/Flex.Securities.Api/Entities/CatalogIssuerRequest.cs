using Flex.Contracts.Domains;
using Flex.Shared.Constants;
using Flex.Shared.Enums.General;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.Securities.Api.Entities
{
    [Table("ISSUERREQUESTS")]
    public class CatalogIssuerRequest : EntityRequestAuditBase<long>
    {
        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public ERequestType Type { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public ERequestStatus Status { get; set; }

        [Column(TypeName = "CLOB")]
        public string DataProposed { get; set; } = Json.EMPTY_JSON;
    }
}
