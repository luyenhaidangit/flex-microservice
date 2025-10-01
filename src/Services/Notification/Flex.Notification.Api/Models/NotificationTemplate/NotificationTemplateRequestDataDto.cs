using Flex.Shared.Constants.Notifications;

namespace Flex.Notification.Api.Models.NotificationTemplate
{
    /// <summary>
    /// DTO for notification template request data.
    /// </summary>
    public class NotificationTemplateRequestDataDto
    {
        /// <summary>
        /// Template key.
        /// </summary>
        public string TemplateKey { get; set; } = default!;

        /// <summary>
        /// Template name.
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Template channel.
        /// </summary>
        public string Channel { get; set; } = TemplateChannel.Email;

        /// <summary>
        /// Template format.
        /// </summary>
        public string Format { get; set; } = TemplateFormat.Html;

        /// <summary>
        /// Template language.
        /// </summary>
        public string Language { get; set; } = Shared.Constants.Common.Language.Vi;

        /// <summary>
        /// Template subject.
        /// </summary>
        public string Subject { get; set; } = default!;

        /// <summary>
        /// Template HTML body.
        /// </summary>
        public string? BodyHtml { get; set; }

        /// <summary>
        /// Template text body.
        /// </summary>
        public string? BodyText { get; set; }

        /// <summary>
        /// Whether template is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Variables specification JSON.
        /// </summary>
        public string? VariablesSpecJson { get; set; }
    }
}
