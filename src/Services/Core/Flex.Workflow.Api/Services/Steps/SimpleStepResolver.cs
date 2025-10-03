using System.Text.Json;
using Flex.Workflow.Api.Entities;

namespace Flex.Workflow.Api.Services.Steps
{
    public class SimpleStepResolver : IStepResolver
    {
        public WorkflowStepsDocument Parse(string? stepsJson)
        {
            if (string.IsNullOrWhiteSpace(stepsJson)) return new WorkflowStepsDocument();
            try
            {
                var doc = JsonSerializer.Deserialize<WorkflowStepsDocument>(stepsJson);
                return doc ?? new WorkflowStepsDocument();
            }
            catch
            {
                return new WorkflowStepsDocument();
            }
        }

        public (int currentStage, int totalStages) GetCurrentStage(IEnumerable<ApprovalAction> actions, WorkflowStepsDocument doc)
        {
            var stages = BuildStages(doc);
            int completed = 0;
            for (int i = 0; i < stages.Count; i++)
            {
                if (StageApproved(i, actions, doc)) completed++;
                else break;
            }
            return (completed, stages.Count);
        }

        public bool IsStageComplete(int stageIndex, IEnumerable<ApprovalAction> actions, WorkflowStepsDocument doc)
        {
            return StageApproved(stageIndex, actions, doc);
        }

        private static List<string> BuildStages(WorkflowStepsDocument doc)
        {
            // Group by group code; steps without group are individual stages in order
            var stages = new List<string>();
            foreach (var step in doc.Steps)
            {
                if (string.IsNullOrWhiteSpace(step.Group))
                {
                    stages.Add($"__{step.Id}");
                }
                else
                {
                    if (!stages.Contains(step.Group)) stages.Add(step.Group);
                }
            }
            return stages;
        }

        private static bool StageApproved(int stageIndex, IEnumerable<ApprovalAction> actions, WorkflowStepsDocument doc)
        {
            var stages = BuildStages(doc);
            if (stageIndex < 0 || stageIndex >= stages.Count) return false;
            var stageKey = stages[stageIndex];

            if (stageKey.StartsWith("__"))
            {
                // single step
                int approvals = actions.Count(a => a.Step == stageIndex + 1 && a.Action.Equals("APPROVE", StringComparison.OrdinalIgnoreCase));
                var stepId = stageKey.Substring(2);
                var step = doc.Steps.FirstOrDefault(s => s.Id == stepId);
                int quorum = step?.Quorum ?? 1;
                return approvals >= quorum;
            }
            else
            {
                // group stage (possibly parallel)
                int approvals = actions.Count(a => a.Step == stageIndex + 1 && a.Action.Equals("APPROVE", StringComparison.OrdinalIgnoreCase));
                var grp = doc.Groups.FirstOrDefault(g => g.Code == stageKey);
                int quorum = grp?.Quorum ?? doc.Steps.Where(s => s.Group == stageKey).Sum(s => Math.Max(1, s.Quorum));
                return approvals >= quorum;
            }
        }
    }
}

