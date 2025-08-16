using Flex.AspNetIdentity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("PERMISSIONS");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("ID");

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("CODE");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("NAME");

            builder.Property(x => x.Description)
                .HasMaxLength(500)
                .HasColumnName("DESCRIPTION");

            builder.Property(x => x.IsAssignable)
                .IsRequired()
                .HasDefaultValue(true)
                .HasColumnName("IS_ASSIGNABLE");

            builder.Property(x => x.SortOrder)
                .IsRequired()
                .HasColumnName("SORT_ORDER");

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasColumnName("IS_ACTIVE");

            builder.Property(x => x.ParentPermissionId)
                .HasColumnName("PARENT_PERMISSION_ID");

            builder.HasIndex(x => x.Code).IsUnique();

            builder.HasOne(x => x.ParentPermission)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentPermissionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
