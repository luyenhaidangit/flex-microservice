using Flex.Workflow.Api.Entities;

namespace Flex.Workflow.Api.Services.Steps
{
    public interface IStepResolver
    {
        // Returns zero-based current stage index and total stages
        (int currentStage, int totalStages) GetCurrentStage(IEnumerable<ApprovalAction> actions, WorkflowStepsDocument doc);

        // Returns true if given stage is complete with newApprovalCount
        bool IsStageComplete(int stageIndex, IEnumerable<ApprovalAction> actions, WorkflowStepsDocument doc);

        WorkflowStepsDocument Parse(string? stepsJson);
    }
}

