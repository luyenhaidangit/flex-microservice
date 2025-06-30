using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.AspNetIdentity.Api.Entities;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.ToTable("ROLE_CLAIMS");

            builder.HasKey(rc => rc.Id);
            builder.Property(rc => rc.Id).HasColumnName("ID");

            builder.Property(rc => rc.RoleId).HasColumnName("ROLE_ID");
            builder.Property(rc => rc.ClaimType).HasColumnName("CLAIM_TYPE").HasMaxLength(100).IsRequired();
            builder.Property(rc => rc.ClaimValue).HasColumnName("CLAIM_VALUE").HasMaxLength(100);
        }
    }
}
