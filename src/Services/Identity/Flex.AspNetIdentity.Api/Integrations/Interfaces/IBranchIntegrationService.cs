using Flex.AspNetIdentity.Api.Models;

namespace Flex.AspNetIdentity.Api.Integrations.Interfaces
{
    /// <summary>
    /// Interface cho Branch Integration Service
    /// </summary>
    public interface IBranchIntegrationService
    {
        /// <summary>
        /// Get list of branches by branch codes
        /// </summary>
        Task<IReadOnlyList<BranchDto>> BatchGetBranchesAsync(IEnumerable<string> codes, CancellationToken ct = default);

        /// <summary>
        /// Get branch information by code
        /// </summary>
        Task<BranchDto?> GetBranchByCodeAsync(string code, CancellationToken ct = default);

        /// <summary>
        /// Check if branch exists
        /// </summary>
        Task<bool> BranchExistsAsync(string code, CancellationToken ct = default);

        /// <summary>
        /// Get all branches list (if endpoint available)
        /// </summary>
        Task<IReadOnlyList<BranchDto>> GetAllBranchesAsync(CancellationToken ct = default);
    }
}
