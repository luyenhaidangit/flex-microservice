namespace Flex.Contracts.Domains.Interfaces
{
    public interface IEntityRequestAuditBase<TId, TEntityId> : IEntityAuditBase<TId>
    {
        TEntityId? EntityId { get; set; }
    }
}
