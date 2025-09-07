using Flex.Shared.SeedWork.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Infrastructure.EntityFrameworkCore.Configurations
{
    public abstract class ApprovalEntityBaseConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
        where TEntity : ApprovalEntityBase<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Id and primary key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .HasColumnName("ID");

            // Common Status column for all approval entities
            builder.Property(x => x.Status)
                   .HasColumnName("STATUS")
                   .HasMaxLength(3)
                   .HasDefaultValue("AUT");

            // Index for Status column
            builder.HasIndex(x => x.Status)
                   .HasDatabaseName($"IX_{typeof(TEntity).Name.ToUpper()}_STATUS");
        }
    }
}
