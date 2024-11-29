using Flex.Customer.Api.Persistence;
using Flex.Customer.Api.Repositories.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flex.Customer.Api.Repositories
{
    public class CustomerRepository : RepositoryQueryBase<Entities.Customer, int, CustomerContext>, ICustomerRepository
    {
        public CustomerRepository(CustomerContext dbContext) : base(dbContext)
        {
        }

        public Task<Entities.Customer> GetCustomerByUserNameAsync(string username) =>
            FindByCondition(x => x.UserName.Equals(username))
                .SingleOrDefaultAsync();
    }
}
