using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.Ordering.Domain.Entities
{
    [Table("ORDERS")]
    public class Order : EntityAuditBase<long>
    {
        [Required]
        [Column("INVESTOR_ID")]
        public long InvestorId { get; set; }

        [Required]
        [Column("SUB_ACCOUNT_ID")]
        public long SubAccountId { get; set; }

        [Required]
        [Column("ORDER_TYPE", TypeName = "VARCHAR2(50)")]
        public string OrderType { get; set; } // BUY, SELL, LIMIT, MARKET

        [Required]
        [Column("QUANTITY", TypeName = "NUMBER(19,4)")]
        public decimal Quantity { get; set; }

        [Column("PRICE", TypeName = "NUMBER(19,4)")]
        public decimal? Price { get; set; }

        [Required]
        [Column("STATUS", TypeName = "VARCHAR2(20)")]
        public string Status { get; set; } = "PENDING"; // PENDING, EXECUTED, CANCELED
    }
}
