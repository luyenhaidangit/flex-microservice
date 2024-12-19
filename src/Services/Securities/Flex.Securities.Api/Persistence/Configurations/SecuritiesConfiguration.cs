using Flex.Securities.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Securities.Api.Persistence.Configurations
{
    public class SecuritiesConfiguration : IEntityTypeConfiguration<CatalogSecurities>
    {
        public void Configure(EntityTypeBuilder<CatalogSecurities> builder)
        {
            // Issuer
            builder.HasOne(e => e.Issuer)
                   .WithMany(e => e.Securities)
                   .HasForeignKey(e => e.IssuerId)
                   .OnDelete(DeleteBehavior.NoAction)
                   .HasConstraintName(null);
        }
    }
}
