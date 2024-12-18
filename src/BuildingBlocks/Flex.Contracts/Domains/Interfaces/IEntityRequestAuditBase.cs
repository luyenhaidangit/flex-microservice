namespace Flex.Contracts.Domains.Interfaces
{
    public interface IEntityRequestAuditBase<T> : IEntityAuditBase<T>
    {
        T? EntityId { get; set; }
    }
}
