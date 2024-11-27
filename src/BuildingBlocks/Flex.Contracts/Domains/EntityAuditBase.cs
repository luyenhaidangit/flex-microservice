using Flex.Contracts.Domains.Interfaces;

namespace Flex.Contracts.Domains
{
    public abstract class EntityAuditBase<T> : EntityBase<T>, IEntityAuditBase<T>
    {
        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset? LastModifiedDate { get; set; }
    }
}
