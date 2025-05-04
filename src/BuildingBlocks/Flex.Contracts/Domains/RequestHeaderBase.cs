using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Contracts.Domains
{
    public abstract class RequestHeaderBase : EntityBase<long>
    {
        [Required]
        [Column("ACTION", TypeName = "VARCHAR2(20)")]
        public string Action { get; set; } = string.Empty;

        [Required]
        [Column("STATUS", TypeName = "VARCHAR2(20)")]
        public string Status { get; set; } = string.Empty;

        [Required]
        [Column("REQUESTED_BY", TypeName = "VARCHAR2(100)")]
        public string RequestedBy { get; set; } = string.Empty;

        [Required]
        [Column("REQUESTED_DATE")]
        public DateTime RequestedDate { get; set; }

        [Column("APPROVE_BY", TypeName = "VARCHAR2(100)")]
        public string? ApproveBy { get; set; }

        [Column("APPROVE_DATE")]
        public DateTime? ApproveDate { get; set; }

        [Column("COMMENTS", TypeName = "VARCHAR2(500)")]
        public string? Comments { get; set; }
    }
}
