using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flex.Contracts.Domains;

namespace Flex.Securities.Api.Entities
{
    [Table("SECURITIES")]
    public class CatalogSecurities : EntityAuditBase<long>
    {
        [Required]
        [Column(TypeName = "VARCHAR2(250)")]
        public string? Symbol { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(10)")]
        public string? TradePlace { get; set; }

        [Column(TypeName = "CLOB")]
        public string? Description { get; set; }
    }
}
