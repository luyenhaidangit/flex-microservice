using Flex.Shared.Constants.Notifications;

namespace Flex.Notification.Api.Models.NotificationTemplate
{
    public class NotificationTemplatePagingDto
    {
        public string TemplateKey { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Channel { get; set; } = TemplateChannel.Email;
        public string Format { get; set; } = TemplateFormat.Html;
        public string Language { get; set; } = Shared.Constants.Common.Language.Vi;
        public string Subject { get; set; } = default!;
        public string? BodyHtml { get; set; }
        public string? BodyText { get; set; }
        public bool IsActive { get; set; } = true;
        public string? VariablesSpecJson { get; set; }
    }
}
