using System.Text.Json;

namespace Flex.Workflow.Api.Services.Policy
{
    public interface IPolicyEvaluator
    {
        PolicyDecision Evaluate(string? policyJson, JsonDocument? input);
    }
}

