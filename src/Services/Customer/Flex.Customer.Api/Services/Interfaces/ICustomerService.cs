namespace Flex.Customer.Api.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IResult> GetCustomerByUsernameAsync(string username);
    }
}
