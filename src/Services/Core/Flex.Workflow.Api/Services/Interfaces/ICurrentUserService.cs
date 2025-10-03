namespace Flex.Workflow.Api.Services.Interfaces
{
    public interface ICurrentUserService
    {
        bool IsAuthenticated();
        string? GetCurrentUsername();
    }
}

