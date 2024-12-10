using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Securities
{
    public class UpdateSecuritiesDto : CreateOrUpdateSecuritiesDto
    {
        [Required]
        public long Id { get; set; }
    }
}
