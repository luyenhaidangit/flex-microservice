namespace Flex.System.Api.Models
{
    public class CreateBranchRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? BranchType { get; set; }
        public bool? IsActive { get; set; }
    }
}
