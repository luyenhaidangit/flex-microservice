using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.Infrastructure.EntityFrameworkCore.Converters;
using Flex.System.Api.Entities;

namespace Flex.System.Api.Persistence.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("BRANCHES");
            builder.HasKey(b => b.Id);

            // Id
            builder.Property(x => x.Id)
                .HasColumnName("ID");

            // Code - bắt buộc, unique
            builder.Property(b => b.Code)
                   .HasColumnName("CODE")
                   .HasMaxLength(50)
                   .IsRequired();

            // Name - bắt buộc
            builder.Property(b => b.Name)
                   .HasColumnName("NAME")
                   .HasMaxLength(256)
                   .IsRequired();

            // BranchType
            builder.Property(b => b.BranchType)
                   .HasColumnName("BRANCH_TYPE")
                   .HasColumnType("NUMBER(10)")
                   .IsRequired();

            // IsActive: bool <-> 'Y'/'N'
            builder.Property(b => b.IsActive)
                   .HasColumnName("IS_ACTIVE")
                   .HasColumnType("CHAR(1)")
                   .HasConversion(new BoolToStringYNConverter())
                   .HasMaxLength(1)
                   .IsRequired()
                   .HasDefaultValueSql("'Y'");

            // Status
            builder.Property(b => b.Status)
                   .HasColumnName("STATUS")
                   .HasMaxLength(50)
                   .IsRequired();

            // Description
            builder.Property(b => b.Description)
                   .HasColumnName("DESCRIPTION")
                   .HasMaxLength(500);

            // Unique index cho Code
            builder.HasIndex(b => b.Code)
                   .HasDatabaseName("UX_BRANCHES_CODE")
                   .IsUnique();
        }
    }
}