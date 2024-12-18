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
            //builder.Property(e => e.Status)
            //    .HasConversion(
            //        v => v.ToValue(),
            //        v => EnumExtension.FromValue<EEntityStatus>(v)
            //    );
        }
    }
}
