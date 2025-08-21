using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.AspNetIdentity.Api.Entities;
using Flex.Infrastructure.EntityFrameworkCore.Configurations;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class RoleRequestConfiguration : RequestBaseConfiguration<RoleRequest, long>, IEntityTypeConfiguration<RoleRequest>
    {
        public override void Configure(EntityTypeBuilder<RoleRequest> builder)
        {
            base.Configure(builder);

            builder.ToTable("ROLE_REQUESTS");
        }
    }
}
