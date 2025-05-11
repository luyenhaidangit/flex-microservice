using System.Runtime.InteropServices;

namespace Flex.Shared.Constants
{
    public class ConfigKeyConstants
    {
        // Configuration file
        public const string ApiConfiguration = "ApiConfiguration";

        public const string JwtSettings = "JwtSettings";

        public const string AllowOrigins = "AllowOrigins";

        public const string IpWhitelist = "IpWhitelist";

        public const string AuthenticationSchemes = "AuthenticationSchemes";
        public class IdentityServer
        {
            public const string Authority = "Authority";

            public const string TokenEndpoint = "TokenEndpoint";
        }

        // Grpc
        public const string GrpcSettings = "GrpcSettings";
        public const string GrpcSettings_SystemUrl = "GrpcSettings:SystemUrl";

        // Configuration database
        public const string AuthMode = "AUTH_MODE";
    }
}
