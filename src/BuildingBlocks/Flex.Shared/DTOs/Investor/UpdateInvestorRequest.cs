using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Investor
{
    public class UpdateInvestorRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }
    }
}
