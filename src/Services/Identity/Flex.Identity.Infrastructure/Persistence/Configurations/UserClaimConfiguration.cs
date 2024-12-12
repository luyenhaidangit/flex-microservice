using Flex.Identity.Infrastructure.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Identity.Infrastructure.Persistence.Configurations
{
    public class UserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        {
            builder.ToTable("UserClaims", Systems.IdentitySchema)
            .HasKey(x => x.Id);

            builder
                .Property(x => x.UserId)
                .IsRequired()
                .HasColumnType("varchar(50)");
        }
    }
}
