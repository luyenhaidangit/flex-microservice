namespace Flex.Shared.DTOs.System.Branch
{
    public class BranchPagingDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Thông tin liên quan đến yêu cầu phê duyệt (nếu có)
        public bool HasPendingRequest { get; set; } = false;
        public string? PendingRequestType { get; set; }
        public string? RequestedBy { get; set; }
        public DateTimeOffset? RequestedDate { get; set; }
    }
}
