using Flex.Shared.Enums.General;

namespace Flex.Shared.DTOs.Securities
{
    public class IssuerPagedDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public EEntityStatus Status { get; set; }
    }
}
