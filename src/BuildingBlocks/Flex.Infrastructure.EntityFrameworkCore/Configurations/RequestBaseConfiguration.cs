using Flex.Shared.SeedWork.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Flex.Infrastructure.EntityFrameworkCore.Configurations
{
    public class RequestBaseConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
        where TEntity : RequestBase<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // ===== ID =====
            builder.HasKey(x => x.Id);

            if (typeof(TKey) == typeof(Guid))
            {
                builder.Property(x => x.Id)
                       .HasColumnName("ID")
                       .HasConversion(
                           v => ((Guid)(object)v).ToByteArray(),  // Guid -> byte[]
                           v => (TKey)(object)new Guid(v)         // byte[] -> Guid
                       )
                       .HasColumnType("RAW(16)")
                       .IsRequired();

                builder.Property(x => x.EntityId)
                       .HasColumnName("ENTITY_ID")
                       .HasConversion(
                           v => ((Guid)(object)v).ToByteArray(),
                           v => (TKey)(object)new Guid(v)
                       )
                       .HasColumnType("RAW(16)")
                       .IsRequired();
            }
            else
            {
                builder.Property(x => x.Id)
                       .HasColumnName("ID")
                       .HasColumnType("NUMBER(19)")
                       .IsRequired();

                builder.Property(x => x.EntityId)
                       .HasColumnName("ENTITY_ID")
                       .HasColumnType("NUMBER(19)")
                       .IsRequired();
            }

            // ===== COMMON FIELDS =====
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
