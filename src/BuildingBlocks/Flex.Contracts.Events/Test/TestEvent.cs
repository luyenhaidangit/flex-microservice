namespace Flex.Contracts.Events.Test
{
    public record TestEvent(Guid MessageId, string TemplateCode, string Recipient, string Content, DateTime CreatedAt);
}
