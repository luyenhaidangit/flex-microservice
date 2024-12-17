using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Securities
{
    public class UpdateSecuritiesDto : CreateOrUpdateSecurityDto
    {
        [Required]
        public long Id { get; set; }
    }
}
