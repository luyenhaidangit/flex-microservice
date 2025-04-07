using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.System.Config
{
    public class SetAuthModeRequest
    {
        [Required]
        public string AuthMode { get; set; } = string.Empty;
    }
}
