using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.System.Api.Entities
{
    [Table("Branches")]
    public class Branch : EntityAuditBase<long>
    {
        [Required]
        [Column("CODE", TypeName = "VARCHAR2(50)")]
        public string Code { get; set; }

        [Required]
        [Column("NAME", TypeName = "NVARCHAR2(200)")]
        public string Name { get; set; }

        [Column("ADDRESS", TypeName = "NVARCHAR2(500)")]
        public string? Address { get; set; }

        [Column("REGION", TypeName = "NVARCHAR2(100)")]
        public string? Region { get; set; }

        [Column("MANAGER", TypeName = "NVARCHAR2(100)")]
        public string? Manager { get; set; }

        [Column("ESTABLISHED_DATE")]
        public DateTime? EstablishedDate { get; set; }

        [Required]
        [Column("STATUS", TypeName = "VARCHAR2(20)")]
        public string Status { get; set; } // "Active", "Inactive", "Pending"
    }
}
