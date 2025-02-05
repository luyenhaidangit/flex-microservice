using Duende.IdentityServer.Models;

namespace Flex.IdentityServer.Api.Configurations
{
    public static class ApiScopes
    {
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("orders.read", "Read access to Orders API"),
                new ApiScope("orders.write", "Write access to Orders API"),
            };
        }
    }
}
