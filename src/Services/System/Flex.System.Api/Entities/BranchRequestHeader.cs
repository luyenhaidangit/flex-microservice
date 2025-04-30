using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.System.Api.Entities
{
    [Table("BRANCH_REQUEST_HEADER")]
    public class BranchRequestHeader : EntityBase<long>
    {
        [Required]
        [Column("ACTION", TypeName = "VARCHAR2(20)")]
        public string Action { get; set; } = default!;  // CREATE / UPDATE / DELETE

        [Required]
        [Column("STATUS", TypeName = "VARCHAR2(10)")]
        public string Status { get; set; } = "UNA";     // UNA / AUT / REJ / CAN

        [Required]
        [Column("MAKER_ID", TypeName = "VARCHAR2(100)")]
        public string MakerId { get; set; } = default!;

        [Required]
        [Column("MAKER_DATE")]
        public DateTime MakerDate { get; set; } = DateTime.UtcNow;

        [Column("CHECKER_ID", TypeName = "VARCHAR2(100)")]
        public string? CheckerId { get; set; }

        [Column("CHECKER_DATE")]
        public DateTime? CheckerDate { get; set; }

        [Column("COMMENTS", TypeName = "NVARCHAR2(500)")]
        public string? Comments { get; set; }
    }
}
