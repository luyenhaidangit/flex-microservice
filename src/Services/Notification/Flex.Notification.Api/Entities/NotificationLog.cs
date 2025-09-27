namespace Flex.Notification.Api.Entities
{
    public class NotificationLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Channel { get; set; } = default!;
        public string Recipient { get; set; } = default!;
        public string TemplateKey { get; set; } = default!;
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? SentAtUtc { get; set; }
        public int RetryCount { get; set; }
        public string? Error { get; set; }
        public Guid CorrelationId { get; set; }
        public string PayloadJson { get; set; } = "{}";

        public static NotificationLog Pending(string channel, string recipient, string templateKey, Guid correlationId, object payload)
            => new()
            {
                Channel = channel,
                Recipient = recipient,
                TemplateKey = templateKey,
                CorrelationId = correlationId,
                PayloadJson = System.Text.Json.JsonSerializer.Serialize(payload)
            };

        public void MarkSent() { Status = "Sent"; SentAtUtc = DateTime.UtcNow; }
        public void MarkFailed(string error) { Status = "Failed"; Error = error; RetryCount++; }
    }
}
