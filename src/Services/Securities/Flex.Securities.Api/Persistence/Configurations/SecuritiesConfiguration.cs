using Flex.Securities.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Securities.Api.Persistence.Configurations
{
    public class SecuritiesConfiguration : IEntityTypeConfiguration<CatalogSecurity>
    {
        public void Configure(EntityTypeBuilder<CatalogSecurity> builder)
        {
        }
    }
}
