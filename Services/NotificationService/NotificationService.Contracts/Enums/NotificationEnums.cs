namespace NotificationService.Contracts.Enums;

public enum NotificationChannel
{
    Push = 0,
    SMS = 1,
    Email = 2
}

public enum NotificationType
{
    RentalCreated = 0,
    RentalStatusChanged = 1,
    PaymentReminder = 2,
    ReturnReminder = 3,
    DeliveryReminder = 4,
    InventoryUnavailable = 5,
    Promotion = 6,
    AccountUpdate = 7,
    AdminReturnDeliverySummary = 8
}

public enum NotificationStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2
}
