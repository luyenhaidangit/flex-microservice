using Flex.Shared.SeedWork.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Infrastructure.EntityFrameworkCore.Configurations
{
    /// <summary>
    /// Base configuration for all RequestViewBase entities
    /// Provides common column mappings for request view properties
    /// </summary>
    public abstract class RequestViewBaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : RequestViewBase
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // ===== View Configuration =====
            this.ConfigureView(builder);

            // ===== Request Base Properties =====
            this.ConfigureRequestBaseProperties(builder);

            // ===== Entity Specific Properties =====
            this.ConfigureEntitySpecificProperties(builder);
        }

        /// <summary>
        /// Configure common RequestViewBase properties
        /// </summary>
        protected virtual void ConfigureRequestBaseProperties(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(x => x.RequestId)
                .HasColumnName("REQUEST_ID");

            builder.Property(x => x.EntityId)
                .HasColumnName("ENTITY_ID");

            builder.Property(x => x.Status)
                .HasColumnName("STATUS");

            builder.Property(x => x.Action)
                .HasColumnName("ACTION");

            builder.Property(x => x.RequestedBy)
                .HasColumnName("REQUESTED_BY");

            builder.Property(x => x.RequestedDate)
                .HasColumnName("REQUESTED_DATE");
        }

        /// <summary>
        /// Configure the database view mapping
        /// Override this method to specify the view name
        /// </summary>
        protected abstract void ConfigureView(EntityTypeBuilder<TEntity> builder);

        /// <summary>
        /// Configure entity-specific properties
        /// Override this method to add specific property mappings
        /// </summary>
        protected abstract void ConfigureEntitySpecificProperties(EntityTypeBuilder<TEntity> builder);
    }
}
