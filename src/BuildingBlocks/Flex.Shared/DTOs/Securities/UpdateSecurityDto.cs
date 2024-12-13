using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Securities
{
    public class UpdateSecurityDto : CreateOrUpdateSecurityDto
    {
        [Required]
        public long Id { get; set; }
    }
}
