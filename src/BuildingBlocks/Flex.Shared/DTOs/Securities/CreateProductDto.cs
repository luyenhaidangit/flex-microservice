using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Securities
{
    public class CreateSecuritiesDto : CreateOrUpdateSecuritiesDto
    {
        [Required]
        public string No { get; set; }
    }
}
