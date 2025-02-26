using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.Investor.Api.Entities
{
    public class Investor : EntityAuditBase<long>
    {
        [Required]
        [Column("NO", TypeName = "VARCHAR2(150)")]
        public string No { get; set; }

        [Required]
        [Column("FULLNAME", TypeName = "VARCHAR2(250)")]
        public string FullName { get; set; }

        [EmailAddress]
        [Column("EMAIL", TypeName = "VARCHAR2(250)")]
        public string? Email { get; set; }

        [Required]
        [Column("PHONE", TypeName = "VARCHAR2(250)")]
        public string Phone { get; set; }

        [Column("ADDRESS", TypeName = "VARCHAR2(500)")]
        public string? Address { get; set; }

        [Column("DATE_OF_BIRTH", TypeName = "DATE")]
        public DateTime? DateOfBirth { get; set; }

        [Column("NATIONALITY", TypeName = "VARCHAR2(100)")]
        public string? Nationality { get; set; }

        [Column("INVESTMENT_PORTFOLIO", TypeName = "VARCHAR2(1000)")]
        public string? InvestmentPortfolio { get; set; }
    }
}
