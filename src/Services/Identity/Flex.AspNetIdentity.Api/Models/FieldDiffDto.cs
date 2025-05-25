namespace Flex.AspNetIdentity.Api.Models
{
    public class FieldDiffDto
    {
        public string Field { get; set; } = default!;
        public string? Original { get; set; }
        public string? Proposed { get; set; }
    }
}
