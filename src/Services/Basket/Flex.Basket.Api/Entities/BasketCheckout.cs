using System.ComponentModel.DataAnnotations;

namespace Flex.Basket.Api.Entities
{
    public class BasketCheckout
    {
        [Required]
        public string InvestorId { get; set; }

        public decimal TotalPrice { get; set; }

        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
