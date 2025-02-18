using Duende.IdentityServer.Models;

namespace Flex.IdentityServer.Api.Configurations
{
    public static class ApiResources
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("resource_api", "My Protected API")
                {
                    Scopes = { "api.read", "api.write" },
                    UserClaims = { "role" }
                }
            };
        }
    }
}
