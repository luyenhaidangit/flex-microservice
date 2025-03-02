using Flex.Contracts.Common.Events;
namespace Flex.Ordering.Domain.Aggregates
{
    public class OrderDeletedEvent : BaseEvent
    {
        public long Id { get; }

        public OrderDeletedEvent(long id)
        {
            Id = id;
        }
    }
}
