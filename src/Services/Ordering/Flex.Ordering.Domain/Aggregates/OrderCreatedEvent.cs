using Flex.Contracts.Common.Events;

namespace Flex.Ordering.Domain.Aggregates
{
    public class OrderCreatedEvent : BaseEvent
    {
        public long Id { get; private set; }
        public long InvestorId { get; private set; }
        public long SubAccountId { get; private set; }
        public string OrderType { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal? Price { get; private set; }
        public string Status { get; private set; }

        public OrderCreatedEvent(long id, long investorId, long subAccountId, string orderType, decimal quantity, decimal? price, string status)
        {
            Id = id;
            InvestorId = investorId;
            SubAccountId = subAccountId;
            OrderType = orderType;
            Quantity = quantity;
            Price = price;
            Status = status;
        }
    }
}
