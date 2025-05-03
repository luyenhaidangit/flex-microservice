namespace Flex.Shared.DTOs.System.Branch
{
    public class GetPendingUpdateRequestResponse
    {
        public long RequestId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
    }
}
