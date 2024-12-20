using Flex.Securities.Api.Entities;
using Flex.Shared.Enums.General;
using Flex.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Securities.Api.Persistence.Configurations
{
    public class IssuerConfiguration : IEntityTypeConfiguration<CatalogIssuer>
    {
        public void Configure(EntityTypeBuilder<CatalogIssuer> builder)
        {
            // ProcessStatus
            builder.Property(e => e.ProcessStatus)
                .HasConversion(
                    v => v.ToValue(),
                    v => EnumExtension.FromValue<EProcessStatus>(v)
            );

            // Securities
            builder.HasMany(e => e.Securities)
                   .WithOne(e => e.Issuer)
                   .HasForeignKey(e => e.IssuerId)
                   .OnDelete(DeleteBehavior.NoAction)
                   .HasConstraintName(null);
        }
    }
}
