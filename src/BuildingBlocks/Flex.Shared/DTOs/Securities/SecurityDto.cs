namespace Flex.Shared.DTOs.Securities
{
    public class SecurityDto
    {
        public long Id { get; set; }

        public string Symbol { get; set; }

        public long IssuerId { get; set; }

        public int TradePlace { get; set; }

        public string? Description { get; set; }
    }
}
