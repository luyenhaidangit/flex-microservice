using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.AspNetIdentity.Api.Entities;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("ROLES");

            // Columns
            builder.Property(r => r.Id).HasColumnName("ID");
            builder.Property(r => r.Name).HasColumnName("NAME");
            builder.Property(r => r.Code).HasColumnName("CODE");
            builder.Property(r => r.Description).HasColumnName("DESCRIPTION");
            builder.Property(r => r.IsActive).HasColumnName("IS_ACTIVE");
            builder.Property(x => x.NormalizedName)
                .HasColumnName("NORMALIZED_NAME")
                .HasColumnType("varchar2(256)");

            // Ignore NormalizedName and ConcurrencyStamp if not present in DB
            builder.Ignore(r => r.NormalizedName);
            builder.Ignore(r => r.ConcurrencyStamp);
        }
    }
}
