namespace Flex.AspNetIdentity.Api.Models.User
{
    public class UserDetailDto : UserPagingDto
    {
        public List<string> Roles { get; set; } = new();
    }

    public class UserChangeHistoryDto
    {
        public long Id { get; set; }
        public string? MakerBy { get; set; }
        public DateTime MakerTime { get; set; }
        public string? ApproverBy { get; set; }
        public DateTime? ApproverTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Changes { get; set; }
    }
}


