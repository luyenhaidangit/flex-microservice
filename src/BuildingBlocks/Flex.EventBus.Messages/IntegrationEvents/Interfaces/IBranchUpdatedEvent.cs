namespace Flex.EventBus.Messages.IntegrationEvents.Interfaces
{
    public interface IBranchUpdatedEvent
    {
        long BranchId { get; }
        string Code { get; }
        string Name { get; }
        int BranchType { get; }
        bool IsActive { get; }
        string? Description { get; }
        DateTime UpdatedDate { get; }
        string UpdatedBy { get; }
    }
}
