namespace Flex.Infrastructure.Workflow.DTOs
{
    public class PendingRequestDtoBase<T>
    {
        public string RequestId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedDate { get; set; } = string.Empty;
        public T? OldData { get; set; }
        public T? NewData { get; set; }
    }
}
