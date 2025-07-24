using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain;
using NotificationService.Contracts.Enums;

namespace NotificationService.Infrastructure.Persistence.Configurations;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.HasKey(nt => nt.Id);
        builder.Property(nt => nt.TitleTemplate).IsRequired().HasMaxLength(200);
        builder.Property(nt => nt.MessageTemplate).IsRequired().HasMaxLength(1000);
        builder.Property(nt => nt.Type).HasConversion<int>();
        builder.Property(nt => nt.Channel).HasConversion<int>();
        builder.Property(nt => nt.IsActive).IsRequired();
    }
}
