namespace Flex.Notification.Api.Services.Interfaces
{
    public interface ICurrentUserService
    {
        bool IsAuthenticated();
        string? GetCurrentUsername();
    }
}

