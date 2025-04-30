using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.System.Api.Entities
{
    [Table("BRANCH_REQUEST_DATA")]
    public class BranchRequestData : EntityBase<long>
    {
        [Required]
        [Column("REQUEST_ID")]
        public long RequestId { get; set; }

        [Required]
        [Column("CODE", TypeName = "VARCHAR2(50)")]
        public string Code { get; set; } = default!;

        [Required]
        [Column("NAME", TypeName = "NVARCHAR2(500)")]
        public string Name { get; set; } = default!;

        [Column("ADDRESS", TypeName = "NVARCHAR2(500)")]
        public string? Address { get; set; }

        [Column("JSON_DATA", TypeName = "CLOB")]
        public string? JsonData { get; set; }
    }
}
