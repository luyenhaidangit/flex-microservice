namespace Flex.EventBus.Messages.IntegrationEvents.Interfaces
{
    public interface IBranchCreatedEvent
    {
        long BranchId { get; }
        string Code { get; }
        string Name { get; }
        int BranchType { get; }
        bool IsActive { get; }
        string? Description { get; }
        DateTime CreatedDate { get; }
        string CreatedBy { get; }
    }
}
