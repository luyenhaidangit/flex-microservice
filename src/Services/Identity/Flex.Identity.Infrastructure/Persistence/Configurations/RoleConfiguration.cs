using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Flex.Identity.Infrastructure.Common.Constants;

namespace Flex.Identity.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.ToTable("Roles", Systems.IdentitySchema).HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder
                .Property(x => x.Name)
                .IsRequired()
                .IsUnicode()
                .HasColumnType("nvarchar(150)")
                .HasMaxLength(150);

            builder.HasData(
                new IdentityRole
                {
                    Name = Roles.Administrator,
                    NormalizedName = Roles.Administrator.ToUpper(),
                    Id = "b6105f01-18f5-433c-91e0-dbd80d27e7f4"
                },
                new IdentityRole
                {
                    Name = Roles.Customer,
                    NormalizedName = Roles.Customer.ToUpper(),
                    Id = "b4365573-ff95-4015-8dd0-adf0650354a2"
                }
            );
        }
    }
}
