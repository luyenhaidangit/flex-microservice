using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.System.Api.Entities
{
    [Table("BranchRequests")]
    public class BranchRequest : EntityAuditBase<long>
    {
        [Column("BRANCH_ID")]
        public long? BranchId { get; set; }

        [Column("REQUEST_TYPE", TypeName = "VARCHAR2(50)")]
        [Required]
        public string RequestType { get; set; } // Create, Update, Close

        [Column("PROPOSED_DATA", TypeName = "CLOB")]
        public string ProposedDataJson { get; set; }

        [Column("REASON", TypeName = "NVARCHAR2(1000)")]
        public string? Reason { get; set; }

        [Required]
        [Column("STATUS", TypeName = "VARCHAR2(20)")]
        public string Status { get; set; } // Pending, Approved, Rejected

        [Column("APPROVED_BY")]
        public long? ApprovedBy { get; set; }

        [Column("APPROVED_DATE")]
        public DateTime? ApprovedDate { get; set; }

        [Column("APPROVAL_COMMENT", TypeName = "NVARCHAR2(1000)")]
        public string? ApprovalComment { get; set; }
    }
}
