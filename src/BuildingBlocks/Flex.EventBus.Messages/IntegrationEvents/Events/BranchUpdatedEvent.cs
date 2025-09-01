using Flex.EventBus.Messages.IntegrationEvents.Interfaces;

namespace Flex.EventBus.Messages.IntegrationEvents.Events
{
    /// <summary>
    /// Event được publish khi Branch được cập nhật trong System service
    /// </summary>
    public record BranchUpdatedEvent : IntegrationBaseEvent, IBranchUpdatedEvent
    {
        public long BranchId { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int BranchType { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; } = default!;
    }
}
