namespace Flex.AspNetIdentity.Api.Models
{
    public class RoleImpactDto
    {
        public string ImpactType { get; set; } = default!; // e.g., "User", "Permission", "Menu"
        public string Name { get; set; } = default!;        // e.g., user name
        public string Code { get; set; } = default!;        // e.g., username, claim name
        public string Description { get; set; } = default!; // mô tả tác động
    }
}
