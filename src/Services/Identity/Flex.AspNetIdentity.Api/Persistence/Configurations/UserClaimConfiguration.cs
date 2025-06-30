using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.AspNetIdentity.Api.Entities;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.ToTable("USER_CLAIMS");
            builder.HasKey(uc => uc.Id);
            builder.Property(uc => uc.Id).HasColumnName("ID");
            builder.Property(uc => uc.UserId).HasColumnName("USER_ID");
            builder.Property(uc => uc.ClaimType).HasColumnName("CLAIM_TYPE").HasMaxLength(256);
            builder.Property(uc => uc.ClaimValue).HasColumnName("CLAIM_VALUE").HasMaxLength(256);
        }
    }
}
