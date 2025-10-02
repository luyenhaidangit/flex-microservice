using System.ComponentModel.DataAnnotations;
using Flex.Shared.Constants.Notifications;

namespace Flex.Notification.Api.Models.NotificationTemplate
{
    public class CreateNotificationTemplateRequestDto
    {
        [Required]
        [StringLength(100)]
        public string TemplateKey { get; set; } = default!;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = default!;

        [Required]
        [StringLength(20)]
        public string Channel { get; set; } = TemplateChannel.Email;

        [Required]
        [StringLength(20)]
        public string Format { get; set; } = TemplateFormat.Html;

        [Required]
        [StringLength(10)]
        public string Language { get; set; } = Shared.Constants.Common.Language.Vi;

        [Required]
        [StringLength(500)]
        public string Subject { get; set; } = default!;

        public string? BodyHtml { get; set; }
        public string? BodyText { get; set; }
        public bool IsActive { get; set; } = true;
        public string? VariablesSpecJson { get; set; }

        public string? Comment { get; set; }
    }
}

