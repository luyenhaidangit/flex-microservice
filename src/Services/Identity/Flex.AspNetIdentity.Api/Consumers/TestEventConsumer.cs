using Flex.Contracts.Events.Test;
using MassTransit;

namespace Flex.AspNetIdentity.Api.Consumers
{
    public class TestEventConsumer : IConsumer<TestEvent>
    {
        private readonly ILogger<TestEventConsumer> _logger;

        public TestEventConsumer(ILogger<TestEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<TestEvent> context)
        {
            _logger.LogInformation("📩 Received TestEvent: {Recipient}", context.Message.Recipient);
            return Task.CompletedTask;
        }
    }
}
