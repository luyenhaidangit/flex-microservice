using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.System.Department
{
    public class UpdateDepartmentRequest
    {
        [Required]
        [MaxLength(400)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        public string? Description { get; set; }

        [Required]
        [RegularExpression("A|E", ErrorMessage = "Status must be 'A' (Active) or 'E' (Expired).")]
        public string Status { get; set; }
    }
}
