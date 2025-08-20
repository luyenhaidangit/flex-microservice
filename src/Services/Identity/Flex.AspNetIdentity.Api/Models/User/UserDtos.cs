namespace Flex.AspNetIdentity.Api.Models.User
{
    public class UserApprovedListItemDto
    {
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public long? BranchId { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserDetailDto : UserApprovedListItemDto
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


