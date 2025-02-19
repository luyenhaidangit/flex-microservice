using Duende.IdentityServer.Models;

namespace Flex.IdentityServer.Api.Configurations
{
    public static class ApiScopes
    {
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("my_api.full_access", "Full access to My API")
            };
        }
    }
}
