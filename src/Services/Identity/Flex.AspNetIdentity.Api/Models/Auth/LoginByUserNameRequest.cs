namespace Flex.AspNetIdentity.Api.Models.Auth
{
    public class LoginByUserNameRequest
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
