using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain;
using NotificationService.Contracts.Enums;

namespace NotificationService.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
        builder.Property(n => n.Status).HasConversion<int>();
        builder.Property(n => n.Channel).HasConversion<int>();
        builder.Property(n => n.Type).HasConversion<int>();
        builder.Property(n => n.CreatedAt).IsRequired();
        builder.Property(n => n.RecipientUserId).IsRequired();
    }
}
