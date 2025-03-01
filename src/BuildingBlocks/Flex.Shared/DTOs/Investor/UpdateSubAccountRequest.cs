using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Investor
{
    public class UpdateSubAccountRequest
    {
        [Required]
        public long Id { get; set; } // ID của tiểu khoản

        [Required]
        [StringLength(50)]
        public string AccountType { get; set; } = string.Empty; // Loại tài khoản

        public decimal Balance { get; set; } = 0; // Số dư cập nhật

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "ACTIVE"; // ACTIVE, BLOCKED, CLOSED
    }
}
