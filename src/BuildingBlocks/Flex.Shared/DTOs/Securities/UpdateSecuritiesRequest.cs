using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Securities
{
    public class UpdateSecuritiesRequest
    {
        [Required]
        public long Id { get; set; }

        [MaxLength(20, ErrorMessage = "Maximum length for Securities Name is 250 characters.")]
        public string? Symbol { get; set; }

        public long? IssuerId { get; set; }

        public string? TradePlace { get; set; }

        public string? Description { get; set; }
    }
}
