namespace Flex.Shared.SeedWork
{
    public class EntityKey<TKey>
    {
        public TKey Key { get; set; }

        public EntityKey(TKey key)
        {
            Key = key;
        }
    }
}
