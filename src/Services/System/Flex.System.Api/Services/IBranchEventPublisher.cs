using Flex.EventBus.Messages.IntegrationEvents.Events;

namespace Flex.System.Api.Services
{
    /// <summary>
    /// Service để publish Branch events
    /// </summary>
    public interface IBranchEventPublisher
    {
        Task PublishBranchCreatedAsync(BranchCreatedEvent branchCreatedEvent);
        Task PublishBranchUpdatedAsync(BranchUpdatedEvent branchUpdatedEvent);
        Task PublishBranchDeletedAsync(BranchDeletedEvent branchDeletedEvent);
    }
}
