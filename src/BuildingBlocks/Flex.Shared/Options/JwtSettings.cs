using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.Options
{
    public class JwtSettings
    {
        [Required]
        public string SecretKey { get; set; }
    }
}
