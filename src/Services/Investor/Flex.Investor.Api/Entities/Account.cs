using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flex.Shared.Enums.Investor;

namespace Flex.Investor.Api.Entities
{
    public class Account : EntityAuditBase<long>
    {
        [Required]
        [Column(TypeName = "VARCHAR2(150)")]
        public string No { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(250)")]
        public string FullName { get; set; }

        [EmailAddress]
        [Column(TypeName = "VARCHAR2(250)")]
        public string? Email { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(250)")]
        public string Phone { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(50)")]
        public EAccountStatus Status { get; set; }
    }
}
