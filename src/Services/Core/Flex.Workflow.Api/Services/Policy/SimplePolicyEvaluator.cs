using System.Globalization;
using System.Text.Json;

namespace Flex.Workflow.Api.Services.Policy
{
    public class SimplePolicyEvaluator : IPolicyEvaluator
    {
        public PolicyDecision Evaluate(string? policyJson, JsonDocument? input)
        {
            if (string.IsNullOrWhiteSpace(policyJson)) return new PolicyDecision { Result = "manual" };

            try
            {
                var doc = JsonSerializer.Deserialize<SimplePolicyDocument>(policyJson);
                if (doc == null || doc.Rules.Count == 0) return new PolicyDecision { Result = "manual" };

                foreach (var rule in doc.Rules)
                {
                    if (Matches(rule, input))
                    {
                        return new PolicyDecision { Result = rule.Then };
                    }
                }
            }
            catch
            {
                // Ignore parsing errors -> manual
            }
            return new PolicyDecision { Result = "manual" };
        }

        private static bool Matches(SimpleRule rule, JsonDocument? input)
        {
            if (input == null || string.IsNullOrWhiteSpace(rule.When)) return false;

            var path = rule.When.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            JsonElement current = input.RootElement;
            foreach (var segment in path)
            {
                if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(segment, out var prop))
                {
                    current = prop;
                }
                else return false;
            }

            var left = current;
            var rightText = rule.Value ?? string.Empty;
            if (TryGetNumber(left, out var leftNum) && double.TryParse(rightText, NumberStyles.Any, CultureInfo.InvariantCulture, out var rightNum))
            {
                return CompareNumbers(leftNum, rightNum, rule.Op);
            }
            else
            {
                var leftStr = left.ToString();
                return CompareStrings(leftStr, rightText, rule.Op);
            }
        }

        private static bool TryGetNumber(JsonElement el, out double value)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Number:
                    return el.TryGetDouble(out value);
                case JsonValueKind.String:
                    return double.TryParse(el.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value);
                default:
                    value = 0; return false;
            }
        }

        private static bool CompareNumbers(double l, double r, string op) => op switch
        {
            "<" => l < r,
            "<=" => l <= r,
            ">" => l > r,
            ">=" => l >= r,
            "==" => Math.Abs(l - r) < 1e-9,
            "!=" => Math.Abs(l - r) >= 1e-9,
            _ => false
        };

        private static bool CompareStrings(string l, string r, string op)
        {
            switch (op)
            {
                case "==": return string.Equals(l, r, StringComparison.OrdinalIgnoreCase);
                case "!=": return !string.Equals(l, r, StringComparison.OrdinalIgnoreCase);
                case "in":
                    return r.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Any(x => string.Equals(x, l, StringComparison.OrdinalIgnoreCase));
                case "notin":
                    return !r.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                             .Any(x => string.Equals(x, l, StringComparison.OrdinalIgnoreCase));
                default:
                    return false;
            }
        }
    }
}

