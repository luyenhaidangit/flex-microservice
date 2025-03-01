using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Investor
{
    public class SubAccountDto
    {
        public long Id { get; set; } // ID của tiểu khoản

        [Required]
        [StringLength(50)]
        public string AccountNo { get; set; } = string.Empty; // Mã số tiểu khoản

        [Required]
        [StringLength(50)]
        public string AccountType { get; set; } = string.Empty; // Loại tài khoản: CASH, MARGIN, DERIVATIVES, BONDS

        public decimal Balance { get; set; } = 0; // Số dư tài khoản

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "ACTIVE"; // Trạng thái tài khoản
    }
}
