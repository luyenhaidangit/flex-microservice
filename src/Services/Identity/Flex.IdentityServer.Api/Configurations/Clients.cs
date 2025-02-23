using Duende.IdentityServer.Models;

namespace Flex.IdentityServer.Api.Configurations
{
    public static class Clients
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // Client dùng Client Credentials Flow (server-to-server)
                new Client
                {
                    ClientId = "server_client",
                    ClientSecrets = { new Secret("server_secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { "openid", "profile", "roles", "my_api.full_access" },
                    AccessTokenLifetime = 3600
                },
            };
        }
    }
}
