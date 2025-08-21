using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.AspNetIdentity.Api.Entities;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable("USER_LOGINS");

            builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

            builder.Property(ul => ul.LoginProvider)
                   .HasColumnName("LOGIN_PROVIDER");

            builder.Property(ul => ul.ProviderKey)
                   .HasColumnName("PROVIDER_KEY");

            builder.Property(ul => ul.ProviderDisplayName)
                   .HasColumnName("PROVIDER_DISPLAY_NAME");

            builder.Property(ul => ul.UserId)
                   .HasColumnName("USER_ID");
        }
    }
}
