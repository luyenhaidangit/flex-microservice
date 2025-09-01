using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.SeedWork.Workflow
{
    /// <summary>
    /// Base DTO for all request views with common fields
    /// </summary>
    public abstract class RequestViewBase
    {
        /// <summary>
        /// Request ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Entity ID that the request is for
        /// </summary>
        public long EntityId { get; set; }

        /// <summary>
        /// Request status (UNAUTHORISED, AUTHORISED, REJECTED)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Request action (CREATE, UPDATE, DELETE)
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// User who created the request
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Date when request was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// User who approved/rejected the request
        /// </summary>
        public string? ApproveBy { get; set; }

        /// <summary>
        /// Date when request was approved/rejected
        /// </summary>
        public DateTime? ApproveDate { get; set; }
    }
}
