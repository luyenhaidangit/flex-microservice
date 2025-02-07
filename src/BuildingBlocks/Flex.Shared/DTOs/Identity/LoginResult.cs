namespace Flex.Shared.DTOs.Identity
{
    public class LoginResult
    {
        public LoginResult()
        {
        }

        public LoginResult(string? accessToken)
        {
            AccessToken = accessToken;
        }   

        public string? AccessToken { get; set; }
    }
}
