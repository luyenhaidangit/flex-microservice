using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.Investor.Api.Entities
{
    public class Investor : EntityAuditBase<long>
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
    }
}
