using Flex.Contracts.Domains.Interfaces;
using Flex.System.Api.Entities;
using Flex.AspNetIdentity.Api.Persistence;

namespace Flex.System.Api.Repositories.Interfaces
{
    public interface IUserRequestDataRepository : IRepositoryBase<UserRequestData, long, IdentityDbContext>
    {
    }
}
