namespace Flex.AspNetIdentity.Api.Models
{
    /// <summary>
    /// DTO cho lịch sử thay đổi vai trò theo format frontend yêu cầu
    /// </summary>
    public class RoleChangeHistoryDto
    {
        public long Id { get; set; }
        public string MakerBy { get; set; } = string.Empty;
        public DateTime MakerTime { get; set; }
        public string? ApproverBy { get; set; } = string.Empty;
        public DateTime? ApproverTime { get; set; }
        public required string Status { get; set; }
        public string? Description { get; set; } = string.Empty;
        public Dictionary<string, object> Changes { get; set; } = new Dictionary<string, object>();
        public RawDataDto RawData { get; set; } = new RawDataDto();
    }
    /// <summary>
    /// DTO cho dữ liệu thô trước và sau
    /// </summary>
    public class RawDataDto
    {
        public Dictionary<string, object> Before { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> After { get; set; } = new Dictionary<string, object>();
    }
} 