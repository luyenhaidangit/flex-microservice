using Flex.Identity.Infrastructure.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Identity.Infrastructure.Persistence.Configurations
{
    public class UserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
        {
            builder.ToTable("UserLogins", Systems.IdentitySchema)
            .HasKey(x => x.UserId);

            builder
                .Property(x => x.UserId)
                .IsRequired()
                .HasColumnType("varchar(50)");
        }
    }
}
