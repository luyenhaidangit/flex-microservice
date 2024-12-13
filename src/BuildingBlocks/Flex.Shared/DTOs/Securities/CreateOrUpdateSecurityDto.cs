using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Securities
{
    public abstract class CreateOrUpdateSecurityDto
    {
        [Required]
        [MaxLength(20, ErrorMessage = "Maximum length for Product Name is 250 characters.")]
        public string Symbol { get; set; }

        [Required]
        public long IssuerId { get; set; }

        [Required]
        public int TradePlace { get; set; }

        public string? Description { get; set; }
    }
}
