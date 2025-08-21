using Flex.Contracts.Domains;

namespace Flex.AspNetIdentity.Api.Entities
{
    public class User : EntityBase<long>
    {
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; } = false;
        public bool TwoFactorEnabled { get; set; } = false;
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; } = false;
        public int AccessFailedCount { get; set; } = 0;

        public string FullName { get; set; } = default!;
        public long BranchId { get; set; }
    }
}
