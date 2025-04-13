namespace Flex.Shared.DTOs.System.Branch
{
    public class BranchDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Region { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime? EstablishedDate { get; set; }
        public string? ManagerName { get; set; }
    }
}
