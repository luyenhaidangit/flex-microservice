using Flex.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Flex.Identity.Api.Persistence
{
    public class IdentityDbContext : IdentityDbContext<User, Role, long>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }
    }
}
