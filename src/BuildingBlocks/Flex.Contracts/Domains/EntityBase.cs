using Flex.Contracts.Domains.Interfaces;

namespace Flex.Contracts.Domains
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        public TKey Id { get; set; }
    }
}
