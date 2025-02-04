using Duende.IdentityServer.Models;

namespace Flex.Identity.Api.Configurations
{
    public static class ApiResources
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("orders", "Orders Service")
                {
                    Scopes = { "orders.read", "orders.write" }
                },
            };
        }
    }
}
