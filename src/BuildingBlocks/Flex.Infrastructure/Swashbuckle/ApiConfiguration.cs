using System.ComponentModel.DataAnnotations;

namespace Flex.Infrastructure.Swashbuckle
{
    /// <summary>
    /// Configuration swagger for the API.
    /// var apiConfiguration = configuration.GetRequiredSection<ApiConfiguration>(ConfigKeyConstants.ApiConfiguration)
    /// </summary>
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
