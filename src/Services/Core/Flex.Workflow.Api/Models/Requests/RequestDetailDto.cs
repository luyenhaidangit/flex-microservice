namespace Flex.Workflow.Api.Models.Requests
{
    public class RequestDetailDto
    {
        public long RequestId { get; set; }
        public string Domain { get; set; } = string.Empty;
        public string WorkflowCode { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string MakerId { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public string? Comments { get; set; }
        public string? BusinessId { get; set; }
        public string? CorrelationId { get; set; }
        public object? Payload { get; set; }
        public List<RequestActionDto> Actions { get; set; } = new();
        public List<RequestAuditDto> Audits { get; set; } = new();
    }

    public class RequestActionDto
    {
        public int Step { get; set; }
        public string Action { get; set; } = string.Empty;
        public string ActorId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Comment { get; set; }
        public string? EvidenceUrl { get; set; }
    }

    public class RequestAuditDto
    {
        public string Event { get; set; } = string.Empty;
        public string ActorId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Metadata { get; set; }
    }
}

