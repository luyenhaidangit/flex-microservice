namespace Flex.Shared.DTOs.System.Branch
{
    public class BranchHistoryDto
    {
        public long Id { get; set; }

        public long BranchId { get; set; }

        public string ChangeType { get; set; } = default!; // Created, Updated, Closed

        public string? OldDataJson { get; set; }

        public string? NewDataJson { get; set; }

        public string? Comment { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public string? CreatedBy { get; set; }
    }
}
