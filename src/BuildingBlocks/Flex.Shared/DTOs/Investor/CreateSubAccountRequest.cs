using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Investor
{
    public class CreateSubAccountRequest
    {
        [Required]
        public long InvestorId { get; set; } // Liên kết với nhà đầu tư

        [Required]
        [StringLength(50)]
        [RegularExpression("^(CASH|MARGIN|DERIVATIVES|BONDS)$", ErrorMessage = "Invalid account type. Allowed values: CASH, MARGIN, DERIVATIVES, BONDS.")]
        public string AccountType { get; set; } = string.Empty; // Loại tài khoản: CASH, MARGIN, DERIVATIVES, BONDS
    }
}
