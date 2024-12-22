using Flex.Shared.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Securities
{
    public class UpdateSecuritiesRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string? Symbol { get; set; }

        [Required]
        [AllowedConstantValues(typeof(AllowedConstantValuesAttribute))]
        public string? TradePlace { get; set; }

        public string? Description { get; set; }
    }
}
