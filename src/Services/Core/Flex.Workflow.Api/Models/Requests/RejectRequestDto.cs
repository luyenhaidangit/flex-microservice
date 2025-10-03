namespace Flex.Workflow.Api.Models.Requests
{
    public class RejectRequestDto
    {
        public string ApproverId { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? EvidenceUrl { get; set; }
    }
}

