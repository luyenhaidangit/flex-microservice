using System.ComponentModel.DataAnnotations;

namespace Flex.Basket.Api.Entities
{
    public class CartItem
    {
        [Required]
        [Range(1, double.PositiveInfinity, ErrorMessage = "The field {0} must be >= {1}.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.1, double.PositiveInfinity, ErrorMessage = "The field {0} must be >= {1}.")]
        public decimal ItemPrice { get; set; }

        public string ItemNo { get; set; }
        public string ItemName { get; set; }
    }
}
