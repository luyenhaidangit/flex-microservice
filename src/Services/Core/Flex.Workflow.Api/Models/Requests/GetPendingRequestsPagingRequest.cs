using Flex.Shared.SeedWork;

namespace Flex.Workflow.Api.Models.Requests
{
    public class GetPendingRequestsPagingRequest : PagingRequest
    {
        public string? Domain { get; set; }
        public string? WorkflowCode { get; set; }
        public string? Action { get; set; }
        public string? Keyword { get; set; }

        protected override Dictionary<string, string> OrderByMappings { get; } = new(StringComparer.OrdinalIgnoreCase)
        {
            ["requestedDate"] = "REQUESTED_DATE",
            ["makerId"] = "MAKER_ID",
            ["workflowCode"] = "WORKFLOW_CODE",
            ["domain"] = "DOMAIN",
            ["action"] = "ACTION"
        };
    }
}
