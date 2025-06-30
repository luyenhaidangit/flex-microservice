using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.AspNetIdentity.Api.Entities;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("USER_ROLES");

            builder.HasKey(ur => new { ur.UserId, ur.RoleId });
            builder.Property(ur => ur.UserId).HasColumnName("USER_ID");
            builder.Property(ur => ur.RoleId).HasColumnName("ROLE_ID");
        }
    }
}
