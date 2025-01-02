using Flex.Contracts.Common.Events;
using Flex.Infrastructure.Common;
using MediatR;
using Serilog;

namespace Flex.MediaR
{
    public static class MediaRExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator,
        List<BaseEvent> domainEvents,
        ILogger logger)
        {
            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
                var data = new SerializeService().Serialize(domainEvent);
                logger.Information($"\n-----\nA domain event has been published!\n" +
                                   $"Event: {domainEvent.GetType().Name}\n" +
                                   $"Data: {data})\n-----\n");
            }
        }
    }
}
