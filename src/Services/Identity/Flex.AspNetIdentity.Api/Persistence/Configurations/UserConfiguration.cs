using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.AspNetIdentity.Api.Entities;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("USERS");

        builder.Property(u => u.Id).HasColumnName("ID");
        builder.Property(u => u.UserName).HasColumnName("USER_NAME");
        builder.Property(u => u.NormalizedUserName).HasColumnName("NORMALIZED_USER_NAME");
        builder.Property(u => u.Email).HasColumnName("EMAIL");
        builder.Property(u => u.NormalizedEmail).HasColumnName("NORMALIZED_EMAIL");
        builder.Property(u => u.EmailConfirmed).HasColumnName("EMAIL_CONFIRMED");
        builder.Property(u => u.PasswordHash).HasColumnName("PASSWORD_HASH");
        builder.Property(u => u.SecurityStamp).HasColumnName("SECURITY_STAMP");
        builder.Property(u => u.ConcurrencyStamp).HasColumnName("CONCURRENCY_STAMP");
        builder.Property(u => u.PhoneNumber).HasColumnName("PHONE_NUMBER");
        builder.Property(u => u.PhoneNumberConfirmed).HasColumnName("PHONE_NUMBER_CONFIRMED");
        builder.Property(u => u.TwoFactorEnabled).HasColumnName("TWO_FACTOR_ENABLED");
        builder.Property(u => u.LockoutEnd).HasColumnName("LOCKOUT_END");
        builder.Property(u => u.LockoutEnabled).HasColumnName("LOCKOUT_ENABLED");
        builder.Property(u => u.AccessFailedCount).HasColumnName("ACCESS_FAILED_COUNT");
        builder.Property(u => u.FullName).HasColumnName("FULL_NAME");
        builder.Property(u => u.BranchId).HasColumnName("BRANCH_ID");
    }
}
