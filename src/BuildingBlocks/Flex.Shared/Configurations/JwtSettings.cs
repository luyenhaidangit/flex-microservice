using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.Configurations
{
    public class JwtSettings
    {
        [Required]
        public string SecretKey { get; set; }
    }
}
