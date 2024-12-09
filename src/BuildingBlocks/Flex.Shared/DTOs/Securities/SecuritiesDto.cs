namespace Flex.Shared.DTOs.Securities
{
    public class SecuritiesDto
    {
        public long Id { get; set; }

        public string No { get; set; }

        public string Symbol { get; set; }

        public string IssuerNo { get; set; }

        public int TradePlace { get; set; }

        public string? Description { get; set; }
    }
}
