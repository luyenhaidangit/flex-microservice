using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace Flex.Security
{
    public class JwtSettings
    {
        [Required, MinLength(32)]
        public string SecretKey { get; set; } = string.Empty;

        [Required]
        public string Issuer { get; set; } = string.Empty;

        [Required]
        public string Audience { get; set; } = string.Empty;

        public int ExpiryInMinutes { get; set; } = 60;

        public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;
    }
}
