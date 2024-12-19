using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Securities.Api.Entities
{
    [Table("ISSUERREQUESTS")]
    public class CatalogIssuerRequest : EntityRequestAuditBase<long, long?>
    {
        [Column(TypeName = "VARCHAR2(250)")]
        public string? Name { get; set; }

        [Column(TypeName = "VARCHAR2(50)")]
        public string? Code { get; set; }

        [Column(TypeName = "CLOB")]
        public string? Description { get; set; }
    }
}
