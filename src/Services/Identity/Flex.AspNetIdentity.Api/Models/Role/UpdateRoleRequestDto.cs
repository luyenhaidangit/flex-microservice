using System.ComponentModel.DataAnnotations;

namespace Flex.AspNetIdentity.Api.Models.Role
{
    public class UpdateRoleRequestDto
    {
        [Required]
        [StringLength(100)]
        public string? Code { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public List<string> Claims { get; set; } = new List<string>();

        public string? Comment { get; set; }
    }
}
