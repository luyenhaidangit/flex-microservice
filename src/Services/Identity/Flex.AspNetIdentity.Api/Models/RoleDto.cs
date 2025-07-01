namespace Flex.AspNetIdentity.Api.Models
{
    public class RoleDto
    {
        public long? Id { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool? IsActive { get; set; }
        public bool HasPendingRequest { get; set; }
        public long? PendingRequestId { get; set; }
        public string? RequestType { get; set; }
        public string? RequestedBy { get; set; }
        public DateTime? RequestedAt { get; set; }
    }
}
