namespace Flex.AspNetIdentity.Api.Models
{
    public class CreateRoleDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public List<string>? Claims { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
    }
}
