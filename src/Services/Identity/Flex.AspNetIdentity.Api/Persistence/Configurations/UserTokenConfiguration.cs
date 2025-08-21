using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.AspNetIdentity.Api.Entities;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.ToTable("USER_TOKENS");

            builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

            builder.Property(ut => ut.UserId)
                   .HasColumnName("USER_ID");

            builder.Property(ut => ut.LoginProvider)
                   .HasColumnName("LOGIN_PROVIDER");

            builder.Property(ut => ut.Name)
                   .HasColumnName("NAME");

            builder.Property(ut => ut.Value)
                   .HasColumnName("VALUE");
        }
    }
}
