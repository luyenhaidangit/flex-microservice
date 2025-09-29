using Flex.Shared.SeedWork.Workflow;

namespace Flex.Notification.Api.Models.NotificationTemplate
{
    /// <summary>
    /// DTO for paging pending notification template requests
    /// </summary>
    public class NotificationTemplatePendingPagingDto : RequestViewBase
    {
        public string TemplateKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
    }
}
