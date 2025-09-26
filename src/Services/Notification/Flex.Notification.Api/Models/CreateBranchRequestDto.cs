namespace Flex.System.Api.Models
{
    public class CreateBranchRequestDto
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int BranchType { get; set; }
        public bool IsActive { get; set; }
    }
}
