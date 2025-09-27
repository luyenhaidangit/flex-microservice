using Flex.Notification.Api.Entities;
using Flex.Shared.Constants.Common;
using Flex.Shared.Constants.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Notification.Api.Persistence.Configurations
{
    public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
    {
        public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
        {
            // ===== Table Configuration =====
            builder.ToTable("NOTIFICATION_TEMPLATES");

            // ===== Primary Key =====
            builder.HasKey(e => e.Id);

            // ===== Column Mappings =====
            builder.Property(e => e.Id)
                .HasColumnName("ID")
                .HasColumnType("RAW(16)")
                .IsRequired();

            builder.Property(e => e.TemplateKey)
                .HasColumnName("TEMPLATE_KEY")
                .HasColumnType("VARCHAR2(100 CHAR)")
                .IsRequired();

            builder.Property(e => e.Name)
                .HasColumnName("NAME")
                .HasColumnType("VARCHAR2(200 CHAR)")
                .IsRequired();

            builder.Property(e => e.Channel)
                .HasColumnName("CHANNEL")
                .HasColumnType("VARCHAR2(20 CHAR)")
                .IsRequired()
                .HasDefaultValue(TemplateChannel.Email);

            builder.Property(e => e.Format)
                .HasColumnName("FORMAT")
                .HasColumnType("VARCHAR2(20 CHAR)")
                .IsRequired()
                .HasDefaultValue(TemplateFormat.Html);

            builder.Property(e => e.Language)
                .HasColumnName("LANGUAGE")
                .HasColumnType("VARCHAR2(10 CHAR)")
                .IsRequired()
                .HasDefaultValue(Language.Vi);

            builder.Property(e => e.Subject)
                .HasColumnName("SUBJECT")
                .HasColumnType("VARCHAR2(500 CHAR)")
                .IsRequired();

            builder.Property(e => e.BodyHtml)
                .HasColumnName("BODY_HTML")
                .HasColumnType("CLOB");

            builder.Property(e => e.BodyText)
                .HasColumnName("BODY_TEXT")
                .HasColumnType("CLOB");

            builder.Property(e => e.IsActive)
                .HasColumnName("IS_ACTIVE")
                .HasColumnType("CHAR(1)")
                .HasConversion<string>()
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(e => e.VariablesSpecJson)
                .HasColumnName("VARIABLES_SPEC_JSON")
                .HasColumnType("CLOB");

            // ===== Indexes =====
            builder.HasIndex(e => e.TemplateKey)
                .HasDatabaseName("IX_NOTIFICATION_TEMPLATES_TEMPLATE_KEY")
                .IsUnique();

            builder.HasIndex(e => new { e.TemplateKey, e.Channel, e.Language })
                .HasDatabaseName("IX_NOTIFICATION_TEMPLATES_KEY_CHANNEL_LANG");

            builder.HasIndex(e => e.Channel)
                .HasDatabaseName("IX_NOTIFICATION_TEMPLATES_CHANNEL");

            builder.HasIndex(e => e.Language)
                .HasDatabaseName("IX_NOTIFICATION_TEMPLATES_LANGUAGE");

            builder.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_NOTIFICATION_TEMPLATES_IS_ACTIVE");
        }
    }
}
