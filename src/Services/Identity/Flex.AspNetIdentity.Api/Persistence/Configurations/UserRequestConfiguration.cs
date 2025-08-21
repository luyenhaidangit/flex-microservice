using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.AspNetIdentity.Api.Entities;
using Flex.Infrastructure.EntityFrameworkCore.Configurations;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class UserRequestConfiguration : RequestBaseConfiguration<UserRequest, long>, IEntityTypeConfiguration<UserRequest>
    {
        public override void Configure(EntityTypeBuilder<UserRequest> builder)
        {
            base.Configure(builder);

            builder.ToTable("USER_REQUESTS");
        }
    }
}


