namespace Flex.Shared.DTOs.Securities
{
    public class IssuerDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<SecuritiesDto>? Securities { get; set; }
    }
}
