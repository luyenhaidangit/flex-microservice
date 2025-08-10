using System.ComponentModel.DataAnnotations;

namespace Flex.System.Api.Models
{
    public class CreateBranchRequestDto
    {
        [Required(ErrorMessage = "Branch code is required")]
        [StringLength(50, ErrorMessage = "Branch code cannot exceed 50 characters")]
        public string Code { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(256, ErrorMessage = "Branch name cannot exceed 256 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        
        public int? BranchType { get; set; }
        
        public bool? IsActive { get; set; }
    }
}
