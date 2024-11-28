using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Products
{
    public class CreateProductDto : CreateOrUpdateProductDto
    {
        [Required]
        public string No { get; set; }
    }
}
