using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Securities
{
    public class CreateSecuritiesRequest
    {
        [Required]
        [MaxLength(20, ErrorMessage = "Maximum length for Securities Name is 250 characters.")]
        public string Symbol { get; set; }

        [Required]
        public long IssuerId { get; set; }

        [Required]
        public string TradePlace { get; set; }

        public string? Description { get; set; }
    }
}
