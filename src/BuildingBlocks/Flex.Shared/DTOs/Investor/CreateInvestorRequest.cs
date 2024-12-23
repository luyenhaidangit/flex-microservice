using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Investor
{
    public class CreateInvestorRequest
    {
        [Required]
        [StringLength(150)]
        public string No { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(250)]
        public string? Email { get; set; }

        [Required]
        [StringLength(250)]
        public string Phone { get; set; } = string.Empty;
    }
}
