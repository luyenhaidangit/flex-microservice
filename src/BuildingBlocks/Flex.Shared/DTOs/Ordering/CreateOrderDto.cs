using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Ordering
{
    public class CreateOrderDto
    {
        [Required]
        public long InvestorId { get; set; } // Mã nhà đầu tư (bắt buộc)

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than zero.")]
        public decimal TotalPrice { get; set; } // Tổng giá trị đơn hàng (bắt buộc)

        [Required]
        [StringLength(250, ErrorMessage = "Full name cannot exceed 250 characters.")]
        public string FullName { get; set; } = string.Empty; // Họ và tên khách hàng (bắt buộc)

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty; // Email khách hàng (bắt buộc)

        [Required]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string Address { get; set; } = string.Empty; // Địa chỉ giao hàng (bắt buộc)

        [StringLength(500, ErrorMessage = "Invoice address cannot exceed 500 characters.")]
        public string? InvoiceAddress { get; set; } // Địa chỉ hóa đơn (không bắt buộc)
    }
}
