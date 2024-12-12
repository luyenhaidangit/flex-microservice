namespace Flex.Identity.Infrastructure.Domains
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        public TKey Id { get; set; }
    }
}
