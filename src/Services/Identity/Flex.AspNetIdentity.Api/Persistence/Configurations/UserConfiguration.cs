using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.AspNetIdentity.Api.Entities;
using Flex.Infrastructure.EntityFrameworkCore.Converters;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("USERS");

            // Keys
            builder.HasKey(u => u.Id);

            // Columns
            builder.Property(u => u.Id).HasColumnName("ID");

            builder.Property(u => u.UserName)
                   .HasColumnName("USER_NAME")
                   .HasMaxLength(256);

            builder.Property(u => u.NormalizedUserName)
                   .HasColumnName("NORMALIZED_USER_NAME")
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(u => u.Email)
                   .HasColumnName("EMAIL")
                   .HasMaxLength(256);

            builder.Property(u => u.NormalizedEmail)
                   .HasColumnName("NORMALIZED_EMAIL")
                   .HasMaxLength(256);

            builder.Property(u => u.EmailConfirmed)
                   .HasColumnName("EMAIL_CONFIRMED")
                   .HasColumnType("CHAR(1)")
                   .HasConversion(new BoolToStringYNConverter())
                   .HasMaxLength(1)
                   .HasDefaultValue(false);

            builder.Property(u => u.PasswordHash).HasColumnName("PASSWORD_HASH");
            builder.Property(u => u.SecurityStamp).HasColumnName("SECURITY_STAMP");
            builder.Property(u => u.ConcurrencyStamp).HasColumnName("CONCURRENCY_STAMP").IsConcurrencyToken();

            builder.Property(u => u.PhoneNumber)
                   .HasColumnName("PHONE_NUMBER")
                   .HasMaxLength(50);

            builder.Property(u => u.PhoneNumberConfirmed)
                   .HasColumnName("PHONE_NUMBER_CONFIRMED")
                   .HasColumnType("CHAR(1)")
                   .HasConversion(new BoolToStringYNConverter())
                   .HasMaxLength(1)
                   .HasDefaultValue(false);

            builder.Property(u => u.TwoFactorEnabled)
                   .HasColumnName("TWO_FACTOR_ENABLED")
                   .HasColumnType("CHAR(1)")
                   .HasConversion(new BoolToStringYNConverter())
                   .HasMaxLength(1)
                   .HasDefaultValue(false);

            builder.Property(u => u.LockoutEnd).HasColumnName("LOCKOUT_END");
            builder.Property(u => u.LockoutEnabled)
                   .HasColumnName("LOCKOUT_ENABLED")
                   .HasColumnType("CHAR(1)")
                   .HasConversion(new BoolToStringYNConverter())
                   .HasMaxLength(1)
                   .HasDefaultValue(false);

            builder.Property(u => u.AccessFailedCount).HasColumnName("ACCESS_FAILED_COUNT");

            builder.Property(u => u.FullName)
                   .HasColumnName("FULL_NAME")
                   .HasMaxLength(250);

            builder.Property(u => u.BranchId).HasColumnName("BRANCH_ID");

            // Indexes (the same convention as Role)
            builder.HasIndex(u => u.NormalizedUserName)
                   .HasDatabaseName("UX_USERS_NORMALIZED_USER_NAME")
                   .IsUnique();

            builder.HasIndex(u => u.NormalizedEmail)
                   .HasDatabaseName("IX_USERS_NORMALIZED_EMAIL");
        }
    }
}
