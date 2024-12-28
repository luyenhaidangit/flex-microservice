using Flex.Shared.Attributes;
using Flex.Shared.Constants.Securities;
using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Securities
{
    public class CreateSecuritiesRequest
    {
        [Required]
        [MaxLength(20, ErrorMessage = "Maximum length for Securities Name is 250 characters.")]
        public string Symbol { get; set; }

        [Required]
        [AllowedConstantValues(typeof(TradePlaceConstants))]
        public string TradePlace { get; set; }

        public string? Description { get; set; }
    }
}
