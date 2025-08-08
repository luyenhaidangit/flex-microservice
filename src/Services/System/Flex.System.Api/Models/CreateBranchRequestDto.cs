namespace Flex.System.Api.Models
{
    public class CreateBranchRequestDto
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int? BranchType { get; set; }
        public bool? IsActive { get; set; }
    }
}
