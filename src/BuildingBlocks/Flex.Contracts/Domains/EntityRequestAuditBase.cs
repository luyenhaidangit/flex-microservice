using Flex.Contracts.Domains.Interfaces;

namespace Flex.Contracts.Domains
{
    public abstract class EntityRequestAuditBase<T> : EntityAuditBase<T>, IEntityRequestAuditBase<T>
    {
        public T? EntityId { get; set; }
    }
}
