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
    }
}
