namespace Flex.System.Api.Models.Branch
{
    public class BranchDetailDto
    {
        public long? Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public string? Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int BranchType { get; set; }
        public string? Status { get; set; } = string.Empty;
    }
}
