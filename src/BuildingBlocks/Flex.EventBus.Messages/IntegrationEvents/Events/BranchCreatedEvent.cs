using Flex.EventBus.Messages.IntegrationEvents.Interfaces;

namespace Flex.EventBus.Messages.IntegrationEvents.Events
{
    /// <summary>
    /// Event được publish khi Branch được tạo mới trong System service
    /// </summary>
    public record BranchCreatedEvent : IntegrationBaseEvent, IBranchCreatedEvent
    {
        public long BranchId { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int BranchType { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = default!;
    }
}
