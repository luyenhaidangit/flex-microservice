using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.System.Api.Entities;

namespace Flex.System.Api.Data.Configurations
{
    public class BranchRequestConfiguration : IEntityTypeConfiguration<BranchRequest>
    {
        public void Configure(EntityTypeBuilder<BranchRequest> builder)
        {
            builder.ToTable("BranchRequests");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(20);
                
            builder.Property(x => x.EntityCode)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20);
                
            builder.Property(x => x.RequestData)
                .IsRequired();
                
            builder.Property(x => x.OriginalData);
        }
    }
}
