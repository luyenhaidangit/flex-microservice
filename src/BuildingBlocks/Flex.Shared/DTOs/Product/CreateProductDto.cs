using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Product
{
    public class CreateProductDto : CreateOrUpdateProductDto
    {
        [Required]
        public string No { get; set; }
    }
}
