namespace Flex.AspNetIdentity.Api.Models.User
{
    /// <summary>
    /// DTO for paging pending user requests
    /// </summary>
    public class UserPendingPagingDto
    {
        // ===== Request information =====
        public long? RequestId { get; set; }

        // ===== User information =====
        public string UserName { get; set; } = default!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string RequestType { get; set; } = default!;
        public string? RequestedBy { get; set; }
        public DateTime? RequestedDate { get; set; }
    }
}
