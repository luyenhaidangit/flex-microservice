using System.ComponentModel.DataAnnotations;

namespace Flex.AspNetIdentity.Api.Integrations.Configurations
{
    public class GrpcSettings
    {
        [Required]
        public string SystemUrl { get; set; } = string.Empty;
    }
}
