namespace Flex.Notification.Api.Models.Channel
{
    public class NotificationMessage
    {
        public required string Recipient { get; init; }  // email/sdt/token
        public required string TemplateKey { get; init; } // "AccountActivation"
        public required string Language { get; init; }    // "vi" | "en"
        public required Dictionary<string, string> Variables { get; init; } // {Name, ActivationLink,...}
        public string? SubjectOverride { get; init; }
    }
}
