namespace Flex.System.Api.Models
{
    public class BranchPendingPagingDto
    {
        public long Id { get; set; }
        public string Action { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string IsActive { get; set; } = default!;
        public int BranchType { get; set; }
        public string RequestType { get; set; } = default!;
    }
}
