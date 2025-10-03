namespace Flex.Workflow.Api.Services.Steps
{
    public class WorkflowStepsDocument
    {
        public List<WorkflowStep> Steps { get; set; } = new();
        public List<WorkflowGroup> Groups { get; set; } = new();
    }

    public class WorkflowStep
    {
        public string Id { get; set; } = string.Empty; // e.g., "1", "2a"
        public string? Name { get; set; }
        public string? Group { get; set; } // e.g., "G2" for parallel group
        public int Quorum { get; set; } = 1; // approvals needed for this step (used if no group quorum)
        public List<string> ApproverRoles { get; set; } = new();
    }

    public class WorkflowGroup
    {
        public string Code { get; set; } = string.Empty; // e.g., G2
        public string Type { get; set; } = "sequential"; // sequential | parallel
        public int Quorum { get; set; } = 1; // approvals needed in group stage
    }
}

