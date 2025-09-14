using Flex.Shared.SeedWork.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Infrastructure.EntityFrameworkCore.Configurations
{
    public abstract class RequestBaseConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
        where TEntity : RequestBase<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Id and primary key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .HasColumnName("ID")
                   .HasColumnType("NUMBER(19)");

            // Common columns for all request entities
            builder.Property(x => x.EntityId)
                   .HasColumnName("ENTITY_ID")
                   .HasColumnType("NUMBER(19)");

            builder.Property(x => x.Action)
                   .HasColumnName("ACTION")
                   .HasColumnType("varchar2(20)")
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasColumnName("STATUS")
                   .HasColumnType("varchar2(10)");

            builder.Property(x => x.RequestedData)
                   .HasColumnName("REQUESTED_DATA")
                   .HasColumnType("clob")
                   .IsRequired();

            builder.Property(x => x.Comments)
                   .HasColumnName("COMMENTS")
                   .HasColumnType("nvarchar2(500)");

            builder.Property(x => x.MakerId)
                   .HasColumnName("MAKER_ID")
                   .HasColumnType("varchar2(256)")
                   .IsRequired();

            builder.Property(x => x.RequestedDate)
                   .HasColumnName("REQUESTED_DATE")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .IsRequired();

            builder.Property(x => x.CheckerId)
                   .HasColumnName("CHECKER_ID")
                   .HasColumnType("varchar2(256)");

            builder.Property(x => x.ApproveDate)
                   .HasColumnName("APPROVE_DATE");
        }
    }
}


