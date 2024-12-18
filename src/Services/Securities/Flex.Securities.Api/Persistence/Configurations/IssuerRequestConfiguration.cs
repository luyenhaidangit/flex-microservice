using Flex.Securities.Api.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Flex.Shared.Enums.General;
using Flex.Shared.Extensions;
using System.Reflection.Emit;

namespace Flex.Securities.Api.Persistence.Configurations
{
    public class IssuerRequestConfiguration : IEntityTypeConfiguration<CatalogIssuerRequest>
    {
        public void Configure(EntityTypeBuilder<CatalogIssuerRequest> builder)
        {
            //builder.Property(e => e.Status)
            //    .HasConversion(
            //        v => v.ToValue(),
            //        v => EnumExtension.FromValue<EProcessStatus>(v)
            //    );

            //builder.Property(e => e.Type)
            //    .HasConversion(
            //        v => v.ToValue(),
            //        v => EnumExtension.FromValue<ERequestType>(v)
            //);

            builder.Property(e => e.EntityId).IsRequired(false);
        }
    }
}
