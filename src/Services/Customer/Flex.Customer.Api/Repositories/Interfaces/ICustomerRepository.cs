using Flex.Contracts.Domains.Interfaces;
using Flex.Customer.Api.Persistence;

namespace Flex.Customer.Api.Repositories.Interfaces
{
    public interface ICustomerRepository : IRepositoryQueryBase<Entities.Customer, int, CustomerContext>
    {
        Task<Entities.Customer> GetCustomerByUserNameAsync(string username);
    }
}
