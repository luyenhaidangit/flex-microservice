using Flex.AspNetIdentity.Api.Models;

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
        Task<IReadOnlyList<BranchDto>> BatchGetBranchesAsync(IEnumerable<long> ids, CancellationToken ct = default);

        /// <summary>
        /// Get branch information by id
        /// </summary>
        Task<BranchDto?> GetBranchByIdAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Check if branch exists
        /// </summary>
        Task<bool> BranchExistsAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Get all branches list (if endpoint available)
        /// </summary>
        Task<IReadOnlyList<BranchDto>> GetAllBranchesAsync(CancellationToken ct = default);
    }
}
