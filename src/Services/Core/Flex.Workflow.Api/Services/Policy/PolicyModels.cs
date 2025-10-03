namespace Flex.Workflow.Api.Services.Policy
{
    public class PolicyDecision
    {
        public string Result { get; set; } = "manual"; // auto | manual | reject
        public string? Reason { get; set; }
    }

    public class SimplePolicyDocument
    {
        public string Mode { get; set; } = "simple";
        public List<SimpleRule> Rules { get; set; } = new();
    }

    public class SimpleRule
    {
        public string When { get; set; } = string.Empty;   // json-path-like, dot notation (e.g., amount)
        public string Op { get; set; } = "<";             // <, <=, >, >=, ==, !=, in, notin
        public string? Value { get; set; }                 // compare string value; numeric parsed if possible
        public string Then { get; set; } = "manual";      // auto | manual | reject
    }
}

