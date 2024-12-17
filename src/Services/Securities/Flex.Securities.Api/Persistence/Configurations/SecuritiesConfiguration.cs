using Flex.Securities.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Securities.Api.Persistence.Configurations
{
    public class SecuritiesConfiguration : IEntityTypeConfiguration<CatalogSecurities>
    {
        public void Configure(EntityTypeBuilder<CatalogSecurities> builder)
        {
        }
    }
}
