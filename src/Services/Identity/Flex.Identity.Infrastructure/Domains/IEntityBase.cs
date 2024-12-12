namespace Flex.Identity.Infrastructure.Domains
{
    public interface IEntityBase<T>
    {
        T Id { get; set; }
    }
}
