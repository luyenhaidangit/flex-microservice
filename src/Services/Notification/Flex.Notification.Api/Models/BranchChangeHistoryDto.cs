namespace Flex.Notification.Api.Models
{
    public class BranchChangeHistoryDto
    {
        public long Id { get; set; }
        public string Operation { get; set; } = default!;
        public string RequestedBy { get; set; } = default!;
        public string? ApprovedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? Comments { get; set; }
    }
}
