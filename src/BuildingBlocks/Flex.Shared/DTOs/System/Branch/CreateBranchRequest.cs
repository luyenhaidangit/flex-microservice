using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.System.Branch
{
    public class CreateBranchRequest
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public string? Comments { get; set; }
    }
}
