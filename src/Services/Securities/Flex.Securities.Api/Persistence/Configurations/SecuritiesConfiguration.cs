using Flex.Securities.Api.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Securities.Api.Persistence.Configurations
{
    public class SecuritiesConfiguration
    {
        public void Configure(EntityTypeBuilder<CatalogSecurities> builder)
        {
            // Index
            builder.HasIndex(x => x.No).IsUnique();
        }
    }
}
