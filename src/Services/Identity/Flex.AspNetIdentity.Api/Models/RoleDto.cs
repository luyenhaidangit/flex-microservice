namespace Flex.AspNetIdentity.Api.Models
{
    public class RoleDto
    {
        public long? Id { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool? IsActive { get; set; }
        public List<ClaimDto>? Claims { get; set; }
        public string? Status { get; set; }
    }
}
