using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.System.Branch
{
    public class UpdateBranchRequest
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public required string Name { get; set; }
        public string? Address { get; set; }
        public string? Comments { get; set; }
    }
}
