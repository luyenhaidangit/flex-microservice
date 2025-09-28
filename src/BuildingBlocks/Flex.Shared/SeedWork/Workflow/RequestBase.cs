using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.SeedWork.Workflow
{
    public class RequestBase<TId> : ApprovalEntityBase<long>
    {
        [Column("ENTITY_ID")]
        public TId EntityId { get; set; } = default!;

        [Required]
        [Column("ACTION", TypeName = "VARCHAR2(20)")]
        public string Action { get; set; } = default!;

        [Required]
        [Column("REQUESTED_DATA", TypeName = "CLOB")]
        public string RequestedData { get; set; } = default!;

        [Column("COMMENTS", TypeName = "NVARCHAR2(500)")]
        public string? Comments { get; set; }

        [Required]
        [Column("MAKER_ID", TypeName = "VARCHAR2(100)")]
        public string MakerId { get; set; } = default!;

        [Required]
        [Column("REQUESTED_DATE")]
        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;

        [Column("CHECKER_ID", TypeName = "VARCHAR2(100)")]
        public string? CheckerId { get; set; }

        [Column("APPROVE_DATE")]
        public DateTime? ApproveDate { get; set; }
    }
}
