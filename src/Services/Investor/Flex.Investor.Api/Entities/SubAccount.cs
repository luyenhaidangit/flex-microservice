using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.Investor.Api.Entities
{
    public class SubAccount : EntityAuditBase<long>
    {
        [Required]
        [Column("INVESTOR_ID")]
        public long InvestorId { get; set; }

        public virtual Investor Investor { get; set; } // Navigation property

        [Required]
        [Column("ACCOUNT_NO", TypeName = "VARCHAR2(50)")]
        public string AccountNo { get; set; }

        [Required]
        [Column("ACCOUNT_TYPE", TypeName = "VARCHAR2(50)")]
        public string AccountType { get; set; } // CASH, MARGIN, DERIVATIVES, BONDS

        [Required]
        [Column("BALANCE", TypeName = "NUMBER(19,4)")]
        public decimal Balance { get; set; } = 0;

        [Required]
        [Column("STATUS", TypeName = "VARCHAR2(20)")]
        public string Status { get; set; } = "ACTIVE"; // ACTIVE, BLOCKED, CLOSED
    }
}
