namespace Flex.AspNetIdentity.Api.Models.Auth
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

