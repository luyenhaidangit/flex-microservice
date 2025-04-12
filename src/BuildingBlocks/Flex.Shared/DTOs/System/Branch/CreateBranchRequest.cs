namespace Flex.Shared.DTOs.System.Branch
{
    public class CreateBranchRequest
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public string? Region { get; set; }
        public string? Manager { get; set; }
        public DateTime? EstablishedDate { get; set; }

        public string? Reason { get; set; }
        public long RequestedBy { get; set; }
    }
}
