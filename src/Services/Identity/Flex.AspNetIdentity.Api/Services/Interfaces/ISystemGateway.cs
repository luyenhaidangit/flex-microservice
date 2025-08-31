using Flex.AspNetIdentity.Api.Models;

namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    public interface ISystemGateway
    {
        Task<IReadOnlyList<BranchDto>> BatchGetBranchesAsync(IEnumerable<string> codes, CancellationToken ct);
    }
}

