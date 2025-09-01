using Flex.EventBus.Messages.IntegrationEvents.Interfaces;

namespace Flex.EventBus.Messages.IntegrationEvents.Events
{
    /// <summary>
    /// Event được publish khi Branch bị xóa trong System service
    /// </summary>
    public record BranchDeletedEvent : IntegrationBaseEvent, IBranchDeletedEvent
    {
        public long BranchId { get; set; }
        public string Code { get; set; } = default!;
        public DateTime DeletedDate { get; set; }
        public string DeletedBy { get; set; } = default!;
    }
}
