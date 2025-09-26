namespace Flex.Notification.Api.Models.Branch
{
    /// <summary>
    /// DTO đơn giản cho filter dropdown - chỉ chứa thông tin cần thiết (Id, Name)
    /// </summary>
    public class BranchFilterDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
