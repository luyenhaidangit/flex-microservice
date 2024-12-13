using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Securities
{
    public class UpdateIssuerDto : CreateOrUpdateIssuerDto
    {
        [Required]
        public long Id { get; set; }
    }
}
