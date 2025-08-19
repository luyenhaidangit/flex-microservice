namespace Flex.System.Api.Models.Branch
{
    public class BranchApprovedItemDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Status { get; set; } = default!;
        public bool IsActive { get; set; }
    }
}
