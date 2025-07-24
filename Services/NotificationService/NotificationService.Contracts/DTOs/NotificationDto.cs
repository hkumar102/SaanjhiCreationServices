using NotificationService.Contracts.Enums;

namespace NotificationService.Contracts.DTOs;

public class NotificationConfig
{
    public bool IsPushEnabled { get; set; } = true;
    public bool IsSmsEnabled { get; set; } = false;
    public bool IsEmailEnabled { get; set; } = false;
}

public class NotificationTemplateDto
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public string TitleTemplate { get; set; } = string.Empty;
    public string MessageTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid RecipientUserId { get; set; }
    public NotificationChannel Channel { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Metadata { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Error { get; set; }
}

