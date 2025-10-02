namespace Flex.Notification.Api.Models.NotificationTemplate
{
    public class ApproveNotificationTemplateRequestDto
    {
        public string? Comment { get; set; }
    }

    public class RejectNotificationTemplateRequestDto
    {
        public string? Reason { get; set; }
    }
}

