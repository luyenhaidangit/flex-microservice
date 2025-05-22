using Flex.Shared.SeedWork.Workflow.Constants;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flex.Contracts.Domains;

namespace Flex.Shared.SeedWork.Workflow
{
    public abstract class RequestBase<TId> : EntityBase<long>
    {
        [Column("ENTITY_ID")]
        public TId EntityId { get; set; } = default!;

        [Required]
        [Column("ACTION", TypeName = "VARCHAR2(20)")]
        public string Action { get; set; } = default!;

        [Required]
        [Column("STATUS", TypeName = "VARCHAR2(10)")]
        public string Status { get; set; } = RequestStatusConstant.Unauthorised;

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

        public void Approve(string checkerId)
        {
            if (Status != RequestStatusConstant.Unauthorised)
                throw new InvalidOperationException("Only UNA requests can be approved.");

            Status = RequestStatusConstant.Authorised;
            CheckerId = checkerId;
            ApproveDate = DateTime.UtcNow;
        }

        public void Reject(string checkerId, string reason)
        {
            if (Status != RequestStatusConstant.Unauthorised)
                throw new InvalidOperationException("Only UNA requests can be rejected.");

            Status = RequestStatusConstant.Rejected;
            CheckerId = checkerId;
            ApproveDate = DateTime.UtcNow;
            Comments = reason;
        }

        public void Cancel(string makerId)
        {
            if (Status != RequestStatusConstant.Unauthorised)
                throw new InvalidOperationException("Only UNA requests can be cancelled.");

            Status = RequestStatusConstant.Cancelled;
            CheckerId = makerId;
            ApproveDate = DateTime.UtcNow;
            Comments = "Cancelled by maker.";
        }
    }
}
