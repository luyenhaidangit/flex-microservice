using Flex.AspNetIdentity.Api.Entities;
using Flex.Infrastructure.EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class BranchCacheConfiguration : IEntityTypeConfiguration<BranchCache>
    {
        public void Configure(EntityTypeBuilder<BranchCache> builder)
        {
            builder.ToTable("BRANCH_CACHE");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Code)
                .HasColumnName("CODE")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("NAME")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.BranchType)
                .HasColumnName("BRANCH_TYPE")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("IS_ACTIVE")
                .HasConversion(new BoolToStringYNConverter())
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("DESCRIPTION")
                .HasMaxLength(500);

            builder.Property(x => x.LastSyncedAt)
                .HasColumnName("LAST_SYNCED_AT")
                .IsRequired();

            builder.Property(x => x.LastSyncedBy)
                .HasColumnName("LAST_SYNCED_BY")
                .HasMaxLength(100);

            // Audit fields
            builder.ConfigureAuditFields();

            // Indexes
            builder.HasIndex(x => x.Code)
                .IsUnique()
                .HasDatabaseName("IDX_BRANCH_CACHE_CODE");

            builder.HasIndex(x => x.IsActive)
                .HasDatabaseName("IDX_BRANCH_CACHE_ACTIVE");
        }
    }
}
