namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    public interface IBranchService
    {
        Task<bool> ValidateBranchExistsAsync(long branchId);
    }
}
