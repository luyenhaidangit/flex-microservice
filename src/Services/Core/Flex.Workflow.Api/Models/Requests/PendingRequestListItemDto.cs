namespace Flex.Workflow.Api.Models.Requests
{
    public class PendingRequestListItemDto
    {
        public long RequestId { get; set; }
        public string Domain { get; set; } = string.Empty;
        public string WorkflowCode { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string MakerId { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public string? Comments { get; set; }
        public string? BusinessId { get; set; }
    }
}

