using Flex.Securities.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Securities.Api.Persistence.Configurations
{
    public class IssuerConfiguration : IEntityTypeConfiguration<CatalogIssuer>
    {
        public void Configure(EntityTypeBuilder<CatalogIssuer> builder)
        {
        }
    }
}
