using Duende.IdentityServer.Models;

namespace Flex.Identity.Api.Configurations
{
    public static class Clients
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client_m2m",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("supersecret".Sha256()) },
                    AllowedScopes = { "orders.read", "orders.write" }
                },

                new Client
                {
                    ClientId = "web_app",
                    ClientName = "Example Web App",
                    ClientSecrets = { new Secret("websecret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                    AllowedScopes = { "openid", "profile", "orders.read" },
                    RequireConsent = false
                }
            };
        }
    }
}
