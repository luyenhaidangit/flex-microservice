using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.Configurations
{
    public partial class ApiConfiguration
    {
        [Required]
        public string ApiName { get; set; }

        [Required]
        public string ApiVersion { get; set; }

        [Url]
        [Required]
        public string IdentityServerBaseUrl { get; set; }

        [Url]
        [Required]
        public string IssuerUri { get; set; }

        [Url]
        [Required]
        public string ApiBaseUrl { get; set; }

        [Required]
        public string ClientId { get; set; }
    }
}
