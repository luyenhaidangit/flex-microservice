using Flex.Contracts.Domains;

namespace Flex.System.Api.Entities
{
    public class SmsType : EntityAuditBase<long>
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int MessageLimit { get; set; }

        public decimal OverLimitFee { get; set; }

        public decimal VatRate { get; set; }

        public int MonthlyPaymentDay { get; set; }

        public string Status { get; set; }
    }
}
