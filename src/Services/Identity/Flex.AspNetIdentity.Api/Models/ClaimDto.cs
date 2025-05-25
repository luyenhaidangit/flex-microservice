namespace Flex.AspNetIdentity.Api.Models
{
    public class ClaimDto
    {
        public string Type { get; set; } = "permission";
        public string Value { get; set; } = default!;
    }
}
