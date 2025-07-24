using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NotificationService.Contracts.DTOs;
using NotificationService.Contracts.Enums;
using NotificationService.Domain;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Application.Notifications.Factories;

namespace NotificationService.Application.Notifications.Commands;

public class SendPushNotificationCommandHandler : IRequestHandler<SendPushNotificationCommand, NotificationDto>
{
    private readonly NotificationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<SendPushNotificationCommandHandler> _logger;
    private readonly NotificationSenderFactory _senderFactory;

    public SendPushNotificationCommandHandler(
        NotificationDbContext db,
        IMapper mapper,
        ILogger<SendPushNotificationCommandHandler> logger,
        NotificationSenderFactory senderFactory)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
        _senderFactory = senderFactory;
    }

    public async Task<NotificationDto> Handle(SendPushNotificationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting SendPushNotificationCommand for RecipientUserId: {RecipientUserId}", request.RecipientUserId);
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            RecipientUserId = request.RecipientUserId,
            Channel = NotificationChannel.Push,
            Type = request.Type,
            Title = request.Title,
            Message = request.Message,
            Metadata = request.Metadata,
            Status = NotificationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var sender = _senderFactory.GetSender(notification.Channel);
        if (sender == null)
        {
            notification.Status = NotificationStatus.Failed;
            notification.Error = "Push notifications are disabled or sender not configured.";
            _logger.LogWarning("Push notification not sent: channel disabled or sender missing for RecipientUserId: {RecipientUserId}", request.RecipientUserId);
        }
        else
        {
            try
            {
                notification.Status = await sender.SendAsync(notification, cancellationToken);
                if (notification.Status == NotificationStatus.Sent)
                {
                    notification.SentAt = DateTime.UtcNow;
                    _logger.LogInformation("Push notification sent successfully to RecipientUserId: {RecipientUserId}", request.RecipientUserId);
                }
                else
                {
                    _logger.LogWarning("Push notification failed to send to RecipientUserId: {RecipientUserId}", request.RecipientUserId);
                }
            }
            catch (Exception ex)
            {
                notification.Status = NotificationStatus.Failed;
                notification.Error = ex.Message;
                _logger.LogError(ex, "Exception while sending push notification to RecipientUserId: {RecipientUserId}", request.RecipientUserId);
            }
        }

        // Persist notification
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogTrace("Notification persisted with Id: {NotificationId}", notification.Id);

        return _mapper.Map<NotificationDto>(notification);
    }
}
