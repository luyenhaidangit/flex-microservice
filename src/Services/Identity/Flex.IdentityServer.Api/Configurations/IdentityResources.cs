using Duende.IdentityServer.Models;
using DuendeIdentityResources = Duende.IdentityServer.Models.IdentityResources;

namespace Flex.IdentityServer.Api.Configurations
{
    public static class IdentityResources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new DuendeIdentityResources.OpenId(),
                new DuendeIdentityResources.Profile(),
                new IdentityResource(
                    name: "roles",
                    displayName: "User roles",
                    userClaims: new[] { "role" }
                )
            };
        }
    }
}
