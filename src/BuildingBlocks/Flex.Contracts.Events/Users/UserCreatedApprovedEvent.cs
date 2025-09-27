namespace Flex.Contracts.Events.Users
{
    public sealed record UserCreatedApprovedEvent(
        Guid UserId,
        string Email,
        string FullName,
        string Language,
        string ActivationLink,
        DateTime ApprovedAtUtc
    );
}
