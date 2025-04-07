using static Flex.Shared.Constants.ConfigValuesConstants;

namespace Flex.System.Api.Validators
{
    public static class ConfigValidator
    {
        public static void ValidateAuthMode(string authMode)
        {
            var allowedModes = new[] { AuthMode.None, AuthMode.LDAP, AuthMode.DB };

            if (string.IsNullOrWhiteSpace(authMode) ||
                !allowedModes.Contains(authMode.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                // Throw HTTP 400 exception
                throw new BadHttpRequestException(
                    $"Invalid AuthMode. Allowed values: {string.Join(", ", allowedModes)}",
                    StatusCodes.Status400BadRequest
                );
            }
        }
    }
}
