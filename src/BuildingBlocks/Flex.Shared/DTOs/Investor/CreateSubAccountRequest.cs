using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Investor
{
    public class CreateSubAccountRequest
    {
        [Required]
        public long InvestorId { get; set; } // Liên kết với nhà đầu tư

        [Required]
        [StringLength(50)]
        public string AccountType { get; set; } = string.Empty; // Loại tài khoản: CASH, MARGIN, DERIVATIVES, BONDS
    }
}
