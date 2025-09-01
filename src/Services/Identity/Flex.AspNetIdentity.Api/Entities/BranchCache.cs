using Flex.Contracts.Domains;

namespace Flex.AspNetIdentity.Api.Entities
{
    /// <summary>
    /// Entity cache Branch data từ System service để tối ưu performance
    /// </summary>
    public class BranchCache : EntityBase<long>
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int BranchType { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public DateTime LastSyncedAt { get; set; }
        public string? LastSyncedBy { get; set; }
    }
}
