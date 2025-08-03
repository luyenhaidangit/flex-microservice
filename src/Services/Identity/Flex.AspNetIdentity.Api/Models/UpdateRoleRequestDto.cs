using System.ComponentModel.DataAnnotations;

namespace Flex.AspNetIdentity.Api.Models
{
    public class UpdateRoleRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public List<ClaimDto>? Claims { get; set; }

        public string? Comment { get; set; }
    }
}
