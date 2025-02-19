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
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api.read", "api.write" },
                    AccessTokenLifetime = 3600
                },
            };
        }
    }
}
