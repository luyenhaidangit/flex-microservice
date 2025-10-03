namespace Flex.Infrastructure.Workflow.Abstractions.DTOs;

public record CreateWorkflowRequest(
    string WorkflowType,
    string EntityId,
    string PayloadJson,
    string? Comment,
    string? CorrelationId
);

