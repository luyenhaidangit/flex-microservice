namespace Flex.Notification.Api.Entities
{
    public class NotificationTemplate
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TemplateKey { get; set; } = default!; // "AccountActivation"
        public string Language { get; set; } = "en";
        public string Subject { get; set; } = default!;
        public string BodyHtml { get; set; } = default!;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
