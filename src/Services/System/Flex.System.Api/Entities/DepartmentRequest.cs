using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.System.Api.Entities
{
    [Table("DEPARTMENT_REQUESTS")]
    public class DepartmentRequest : EntityAuditBase<long>
    {
        [Column("DEPARTMENT_ID")]
        public long? DepartmentId { get; set; } // NULL nếu là tạo mới

        [Required]
        [Column("NAME", TypeName = "VARCHAR2(400)")]
        public string Name { get; set; }

        [Column("ADDRESS", TypeName = "VARCHAR2(100)")]
        public string? Address { get; set; }

        [Column("DESCRIPTION", TypeName = "CLOB")]
        public string? Description { get; set; }

        [Required]
        [Column("ACTION_TYPE", TypeName = "VARCHAR2(10)")]
        public string ActionType { get; set; } // CREATE / UPDATE / DELETE

        [Required]
        [Column("REQUEST_STATUS", TypeName = "VARCHAR2(20)")]
        public string RequestStatus { get; set; } // PENDING / APPROVED / REJECTED / CANCELED

        [Column("APPROVED_BY")]
        public long? ApprovedBy { get; set; }

        [Column("APPROVED_AT")]
        public DateTime? ApprovedAt { get; set; }

        [Column("NOTE", TypeName = "CLOB")]
        public string? Note { get; set; } // Ghi chú duyệt/từ chối
    }
}
