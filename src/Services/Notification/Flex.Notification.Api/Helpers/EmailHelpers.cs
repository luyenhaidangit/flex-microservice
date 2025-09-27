using System.Text.RegularExpressions;

namespace Flex.Notification.Api.Helpers
{
    public static class EmailHelpers
    {
        public static string RenderTemplate(string templateContent, IDictionary<string, string> variables)
        {
            return Regex.Replace(templateContent, "\\{(\\w+)\\}", m =>
            {
                var key = m.Groups[1].Value;
                return variables.TryGetValue(key, out var val) ? val : m.Value;
            });
        }
    }
}
