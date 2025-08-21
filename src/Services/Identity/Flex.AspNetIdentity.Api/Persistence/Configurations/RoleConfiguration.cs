using Flex.AspNetIdentity.Api.Entities;
using Flex.Infrastructure.EntityFrameworkCore.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("ROLES");

            // Keys
            builder.HasKey(r => r.Id);

            // Columns
            builder.Property(r => r.Id)
                    .HasColumnName("ID");

            builder.Property(r => r.Name)
                   .HasColumnName("NAME").HasMaxLength(256);

            builder.Property(r => r.NormalizedName)
                   .HasColumnName("NORMALIZED_NAME")
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(r => r.Code)
                   .HasColumnName("CODE")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(r => r.Description)
                   .HasColumnName("DESCRIPTION");

            builder.Property(r => r.Status)
                   .HasColumnName("STATUS")
                   .HasMaxLength(50);

            builder.Property(r => r.IsActive)
                   .HasColumnName("IS_ACTIVE")
                   .HasColumnType("CHAR(1)")
                   .HasConversion(new BoolToStringYNConverter())
                   .HasMaxLength(1)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(r => r.ConcurrencyStamp)
                   .HasColumnName("CONCURRENCY_STAMP")
                   .IsConcurrencyToken();

            // Unique indexes theo chuẩn Identity
            builder.HasIndex(r => r.NormalizedName)
                   .HasDatabaseName("UX_ROLES_NORMALIZED_NAME")
                   .IsUnique();

            builder.HasIndex(r => r.Code)
                   .HasDatabaseName("UX_ROLES_CODE")
                   .IsUnique();
        }
    }
}
