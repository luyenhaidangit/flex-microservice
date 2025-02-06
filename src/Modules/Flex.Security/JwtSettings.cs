using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace Flex.Security
{
    public class JwtSettings
    {
        [Required]
        public string SecretKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int ExpiryInMinutes { get; set; } = 60;

        public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;
    }
}
