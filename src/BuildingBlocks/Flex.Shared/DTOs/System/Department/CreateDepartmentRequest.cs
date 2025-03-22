using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.System.Department
{
    public class CreateDepartmentRequest
    {
        [Required]
        [MaxLength(400)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        public string? Description { get; set; }
    }
}
