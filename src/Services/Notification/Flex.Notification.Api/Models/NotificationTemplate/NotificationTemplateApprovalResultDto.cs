using System;

namespace Flex.Notification.Api.Models.NotificationTemplate
{
    public class NotificationTemplateApprovalResultDto
    {
        public long RequestId { get; set; }
        public string RequestType { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string ApprovedBy { get; set; } = default!;
        public DateTime ApprovedDate { get; set; }
        public string? Comment { get; set; }
        public Guid? CreatedTemplateId { get; set; }
    }
}

