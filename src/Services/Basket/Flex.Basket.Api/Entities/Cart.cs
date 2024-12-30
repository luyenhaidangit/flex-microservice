namespace Flex.Basket.Api.Entities
{
    public class Cart
    {
        public string InvestorId { get; set; }

        public string Email { get; set; }

        public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);

        public DateTimeOffset LastModifiedDate { get; set; } = DateTimeOffset.UtcNow;

        public virtual List<CartItem> Items { get; set; } = new List<CartItem>();

        public string? JobId { get; set; }
    }
}
