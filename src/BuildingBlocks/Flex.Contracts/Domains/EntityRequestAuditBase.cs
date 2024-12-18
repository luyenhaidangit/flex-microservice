using Flex.Contracts.Domains.Interfaces;

namespace Flex.Contracts.Domains
{
    public abstract class EntityRequestAuditBase<TId, TEntityId> : EntityAuditBase<TId>, IEntityRequestAuditBase<TId, TEntityId>
    {
        public TEntityId? EntityId { get; set; }
    }
}
