using System.ComponentModel.DataAnnotations;
using Flex.Shared.SeedWork.Workflow.Constants;

namespace Flex.Shared.DTOs.System.Branch 
{
    public class BranchRequestDTO
    {
        [Required(ErrorMessage = "Request type is required")]
        [RegularExpression(
            $"^({RequestTypeConstant.Create}|{RequestTypeConstant.Update}|{RequestTypeConstant.Delete})$",
            ErrorMessage = "Invalid request type. Must be Create, Update or Delete"
        )]
        public string RequestType { get; set; }

        [Required(ErrorMessage = "Branch code is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Branch code must be between 3 and 20 characters")]
        [RegularExpression("^[a-zA-Z0-9-_]+$", ErrorMessage = "Branch code can only contain letters, numbers, hyphens and underscores")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Branch name must be between 3 and 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }
    }
}
