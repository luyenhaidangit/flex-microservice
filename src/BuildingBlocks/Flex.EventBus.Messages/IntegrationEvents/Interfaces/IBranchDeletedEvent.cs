namespace Flex.EventBus.Messages.IntegrationEvents.Interfaces
{
    public interface IBranchDeletedEvent
    {
        long BranchId { get; }
        string Code { get; }
        DateTime DeletedDate { get; }
        string DeletedBy { get; }
    }
}
