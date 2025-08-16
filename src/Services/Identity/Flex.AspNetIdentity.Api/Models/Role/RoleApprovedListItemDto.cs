namespace Flex.AspNetIdentity.Api.Models.Role
{
    /// <summary>
    /// DTO for paging approved roles
    /// </summary>
    public class RoleApprovedListItemDto
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
