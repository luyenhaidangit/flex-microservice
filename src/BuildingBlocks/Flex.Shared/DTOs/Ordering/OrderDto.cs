using Flex.Shared.Enums.Order;

namespace Flex.Shared.DTOs.Ordering
{
    public class OrderDto
    {
        public long OrderId { get; set; } // ID của đơn hàng (Primary Key)

        public long InvestorId { get; set; } // Mã nhà đầu tư

        public decimal TotalPrice { get; set; } // Tổng giá trị đơn hàng

        public string FullName { get; set; } = string.Empty; // Họ và tên khách hàng

        public string Email { get; set; } = string.Empty; // Email khách hàng

        public string Address { get; set; } = string.Empty; // Địa chỉ giao hàng

        public string? InvoiceAddress { get; set; } // Địa chỉ hóa đơn (có thể null)
    }
}
