using Flex.Identity.Infrastructure.Common.Constants;
using Flex.Identity.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Identity.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", Systems.IdentitySchema)
            .HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.HasIndex(x => x.Email);
            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .ValueGeneratedNever()
                ;

            builder.Property(x => x.NormalizedEmail)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                ;

            builder.Property(x => x.UserName)
                .IsRequired()
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                ;

            builder.Property(x => x.NormalizedUserName)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                ;

            builder.Property(x => x.PhoneNumber)
                .IsUnicode(false)
                .HasColumnType("varchar(20)")
                .HasMaxLength(20);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                ;
            builder.Property(x => x.LastName)
                .IsRequired()
                .HasColumnType("varchar(150)")
                .HasMaxLength(150)
                ;
        }
    }
}
