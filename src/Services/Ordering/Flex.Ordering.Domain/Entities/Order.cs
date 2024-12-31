using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.Ordering.Domain.Entities
{
    [Table("ORDERS")]
    public class Order : EntityAuditBase<long>
    {
        [Required]
        [Column("INVESTORID", TypeName = "NUMBER")]
        public long InvestorId { get; set; }

        [Required]
        [Column("TOTALPRICE", TypeName = "NUMBER(18, 2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Column("FULLNAME", TypeName = "VARCHAR2(250)")]
        public string? FullName { get; set; }

        [Required]
        [Column("EMAIL", TypeName = "VARCHAR2(250)")]
        public string? Email { get; set; }

        [Required]
        [Column("ADDRESS", TypeName = "VARCHAR2(500)")]
        public string? Address { get; set; }

        [Column("INVOICEADDRESS", TypeName = "VARCHAR2(500)")]
        public string? InvoiceAddress { get; set; }
    }
}
