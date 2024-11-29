using AutoMapper;
using Flex.Customer.Api.Repositories.Interfaces;
using Flex.Customer.Api.Services.Interfaces;
using Flex.Shared.DTOs.Customer;

namespace Flex.Customer.Api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IResult> GetCustomerByUsernameAsync(string username)
        {
            var entity = await _repository.GetCustomerByUserNameAsync(username);
            var result = _mapper.Map<CustomerDto>(entity);

            return Results.Ok(result);
        }
    }
}
