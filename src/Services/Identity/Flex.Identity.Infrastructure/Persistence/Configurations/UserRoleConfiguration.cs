using Flex.Identity.Infrastructure.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Identity.Infrastructure.Persistence.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.ToTable("UserRoles", Systems.IdentitySchema)
            .HasKey(x => new { x.UserId, x.RoleId });

            builder
                .Property(x => x.UserId)
                .IsRequired()
                .HasColumnType("varchar(50)");
            builder
                .Property(x => x.RoleId)
                .IsRequired()
                .HasColumnType("varchar(50)");
        }
    }
}
