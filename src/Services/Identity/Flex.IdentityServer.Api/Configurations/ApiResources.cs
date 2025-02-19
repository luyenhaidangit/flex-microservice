using Duende.IdentityServer.Models;

namespace Flex.IdentityServer.Api.Configurations
{
    public static class ApiResources
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("my_api", "My Protected API")
                {
                    Scopes = { "my_api.full_access" }
                }
            };
        }
    }
}
