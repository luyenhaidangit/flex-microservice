namespace Flex.AspNetIdentity.Api.Models.Role
{
    /// <summary>
    /// DTO for paging approved roles
    /// </summary>
    public class RoleApprovedListItemDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
