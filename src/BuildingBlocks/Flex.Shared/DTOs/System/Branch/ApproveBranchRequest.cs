namespace Flex.Shared.DTOs.System.Branch
{
    public class ApproveBranchRequest
    {
        public long RequestId { get; set; }

        public string? Comment { get; set; }

        public string RequestType { get; set; }

        public string ApprovedBy { get; set; }
    }
}
