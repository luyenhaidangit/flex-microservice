using Flex.AspNetIdentity.Api.Models.Branch;

namespace Flex.AspNetIdentity.Api.Integrations.Interfaces
{
    /// <summary>
    /// Interface cho Branch Integration Service
    /// </summary>
    public interface IBranchIntegrationService
    {
        /// <summary>
        /// Get list of branches by branch ids
        /// </summary>
        Task<IReadOnlyList<BranchLookupDto>> BatchGetBranchesAsync(IEnumerable<long> ids, CancellationToken ct = default);

        /// <summary>
        /// Get branch information by id
        /// </summary>
        Task<BranchLookupDto?> GetBranchByIdAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Check if branch exists
        /// </summary>
        Task<bool> BranchExistsAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Get all branches list (if endpoint available)
        /// </summary>
        Task<IReadOnlyList<BranchLookupDto>> GetAllBranchesAsync(CancellationToken ct = default);
    }
}
