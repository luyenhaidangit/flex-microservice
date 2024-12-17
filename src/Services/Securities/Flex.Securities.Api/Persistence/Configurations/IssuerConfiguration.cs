using Flex.Securities.Api.Entities;
using Flex.Shared.Enums.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Securities.Api.Persistence.Configurations
{
    public class IssuerConfiguration : IEntityTypeConfiguration<CatalogIssuer>
    {
        public void Configure(EntityTypeBuilder<CatalogIssuer> builder)
        {
            builder.Property(e => e.Status)
                .HasConversion(
                    v => ConvertEnumToString(v),
                    v => ConvertStringToEnum(v)
                );
        }

        private string ConvertEnumToString(EEntityStatus status)
        {
            if (status == EEntityStatus.PENDING) return "P";
            if (status == EEntityStatus.ACTIVE) return "A";
            if (status == EEntityStatus.INACTIVE) return "I";
            if (status == EEntityStatus.DELETED) return "D";
            throw new ArgumentException("Invalid Status");
        }

        private EEntityStatus ConvertStringToEnum(string value)
        {
            if (value == "P") return EEntityStatus.PENDING;
            if (value == "A") return EEntityStatus.ACTIVE;
            if (value == "I") return EEntityStatus.INACTIVE;
            if (value == "D") return EEntityStatus.DELETED;
            throw new ArgumentException("Invalid Status in Database");
        }
    }
}
