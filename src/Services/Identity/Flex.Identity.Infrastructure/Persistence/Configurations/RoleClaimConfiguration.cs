using Flex.Identity.Infrastructure.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Identity.Infrastructure.Persistence.Configurations
{
    public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            builder.ToTable("RoleClaims", Systems.IdentitySchema)
           .HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .IsRequired()
                .HasColumnType("varchar(50)");
        }
    }
}
