using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flex.Contracts.Domains;

namespace Flex.System.Api.Entities
{
    [Table("BRANCH_MASTER")]
    public class BranchMaster : EntityBase<long>
    {

        [Required]
        [Column("CODE", TypeName = "VARCHAR2(50)")]
        public string Code { get; set; }

        [Required]
        [Column("NAME", TypeName = "NVARCHAR2(500)")]
        public string Name { get; set; }

        [Column("ADDRESS", TypeName = "NVARCHAR2(500)")]
        public string? Address { get; set; }

        [Required]
        [Column("STATUS", TypeName = "VARCHAR2(20)")]
        public string Status { get; set; }
    }
}
