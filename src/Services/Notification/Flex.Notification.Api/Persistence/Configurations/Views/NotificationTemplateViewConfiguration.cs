using Flex.Notification.Api.Entities.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Notification.Api.Persistence.Configurations.Views
{
    public class NotificationTemplateViewConfiguration : IEntityTypeConfiguration<NotificationTemplateRequestView>
    {
        public void Configure(EntityTypeBuilder<NotificationTemplateRequestView> builder)
        {
            // ===== View Configuration =====
            builder.ToView("V_NOTIFICATION_TEMPLATE_REQUESTS");
            builder.HasNoKey();

            // ===== Request Base Properties =====
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

            // ===== Notification Template Properties =====
            builder.Property(x => x.TemplateKey)
                .HasColumnName("TEMPLATE_KEY");

            builder.Property(x => x.Name)
                .HasColumnName("NAME");

            builder.Property(x => x.Channel)
                .HasColumnName("CHANNEL");

            builder.Property(x => x.Format)
                .HasColumnName("FORMAT");

            builder.Property(x => x.Language)
                .HasColumnName("LANGUAGE");

            builder.Property(x => x.Subject)
                .HasColumnName("SUBJECT");

            builder.Property(x => x.BodyHtml)
                .HasColumnName("BODY_HTML");

            builder.Property(x => x.BodyText)
                .HasColumnName("BODY_TEXT");

            builder.Property(x => x.IsActive)
                .HasColumnName("IS_ACTIVE");

            builder.Property(x => x.VariablesSpecJson)
                .HasColumnName("VARIABLES_SPEC_JSON");
        }
    }
}
