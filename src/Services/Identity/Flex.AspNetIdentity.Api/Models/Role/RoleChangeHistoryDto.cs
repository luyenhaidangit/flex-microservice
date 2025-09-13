namespace Flex.AspNetIdentity.Api.Models.Role
{
    public class RoleChangeHistoryDto
    {
        public long Id { get; set; }
        public required string MakerBy { get; set; }
        public DateTime MakerTime { get; set; }
        public string? ApproverBy { get; set; } = string.Empty;
        public DateTime? ApproverTime { get; set; }
        public required string Status { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required string Changes { get; set; }
    }
} 