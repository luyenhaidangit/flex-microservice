using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flex.Contracts.Domains;
using Flex.Shared.Enums.Securities;

namespace Flex.Securities.Api.Entities
{
    [Table("SECURITIES")]
    public class CatalogSecurities : EntityAuditBase<long>
    {
        [Required]
        [Column(TypeName = "VARCHAR2(250)")]
        public string Symbol { get; set; }

        [Required]
        [Column(TypeName = "NUMBER(10)")]
        public long IssuerId { get; set; }

        [Required]
        [Column(TypeName = "NUMBER(10)")]
        public ETradePlace TradePlace { get; set; }

        [Column(TypeName = "CLOB")]
        public string? Description { get; set; }

        #region Navigation
        //[ForeignKey("IssuerId")]
        public virtual CatalogIssuer Issuer { get; set; }
        #endregion
    }
}
