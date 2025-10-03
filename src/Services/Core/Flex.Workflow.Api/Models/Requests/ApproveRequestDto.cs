namespace Flex.Workflow.Api.Models.Requests
{
    public class ApproveRequestDto
    {
        public string ApproverId { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public string? EvidenceUrl { get; set; }
    }
}

