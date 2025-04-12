namespace Flex.Shared.DTOs.System.Branch
{
    public class CloseBranchRequest
    {
        public long BranchId { get; set; }
        public string? Reason { get; set; }
        public long RequestedBy { get; set; }
    }
}
