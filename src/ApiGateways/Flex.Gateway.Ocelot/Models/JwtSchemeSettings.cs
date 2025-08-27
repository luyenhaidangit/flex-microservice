namespace Flex.Gateway.Ocelot.Models
{
    public class JwtSchemeSettings
    {
        public string Authority { get; set; } = default!;
        public string Audience { get; set; } = default!;
    }
}
