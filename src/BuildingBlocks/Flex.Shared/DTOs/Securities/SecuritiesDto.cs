namespace Flex.Shared.DTOs.Securities
{
    public class SecuritiesDto
    {
        public long Id { get; set; }

        public string Symbol { get; set; }

        public long IssuerId { get; set; }

        public string TradePlace { get; set; }

        public string? Description { get; set; }
    }
}
