namespace Flex.Workflow.Api.Models.Requests
{
    public class CreateApprovalRequestDto
    {
        public string Domain { get; set; } = string.Empty;
        public string WorkflowCode { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // CREATE/UPDATE/DELETE
        public string? BusinessId { get; set; }
        public string? CorrelationId { get; set; }
        public string MakerId { get; set; } = string.Empty;
        public string? Comments { get; set; }
        public object? Payload { get; set; }
    }
}

