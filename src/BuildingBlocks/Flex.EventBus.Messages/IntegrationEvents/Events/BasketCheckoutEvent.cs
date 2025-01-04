using Flex.EventBus.Messages.IntegrationEvents.Interfaces;

namespace Flex.EventBus.Messages.IntegrationEvents.Events
{
    public record BasketCheckoutEvent : IntegrationBaseEvent, IBasketCheckoutEvent
    {
        public string UserName { get; set; }
        public decimal TotalPrice { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string InvoiceAddress { get; set; }
    }
}
