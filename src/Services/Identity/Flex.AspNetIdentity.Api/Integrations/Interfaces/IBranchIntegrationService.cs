using Flex.AspNetIdentity.Api.Models.Branch;

namespace Flex.AspNetIdentity.Api.Integrations.Interfaces
{
    /// <summary>
    /// Interface cho Branch Integration Service
    /// </summary>
    public interface IBranchIntegrationService
    {
        Task<IReadOnlyList<BranchLookupDto>> BatchGetBranchesAsync(IEnumerable<long> ids, CancellationToken ct = default);
        Task<BranchLookupDto?> GetBranchByIdAsync(long id, CancellationToken ct = default);
    }
}
