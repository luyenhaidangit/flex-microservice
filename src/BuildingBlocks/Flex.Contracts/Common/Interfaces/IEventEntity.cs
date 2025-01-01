using Flex.Contracts.Common.Events;
using Flex.Contracts.Domains.Interfaces;

namespace Flex.Contracts.Common.Interfaces
{
    public interface IEventEntity
    {
        void AddDomainEvent(BaseEvent domainEvent);
        void RemoveDomainEvent(BaseEvent domainEvent);
        void ClearDomainEvents();
        IReadOnlyCollection<BaseEvent> DomainEvents();
    }

    public interface IEventEntity<T> : IEntityBase<T>, IEventEntity
    {
    }
}
