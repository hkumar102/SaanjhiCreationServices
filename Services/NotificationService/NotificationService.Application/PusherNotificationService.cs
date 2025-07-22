using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NotificationService.Application;

public class PusherNotificationService : INotificationService
{
    private readonly ILogger<PusherNotificationService> _logger;

    public PusherNotificationService(ILogger<PusherNotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyAdminAsync(string eventName, object payload)
    {
        // TODO: Integrate with Pusher SDK
        _logger.LogInformation($"Pusher event '{eventName}' sent to admin with payload: {payload}");
        return Task.CompletedTask;
    }

    public Task NotifyCustomerSmsAsync(string phoneNumber, string message)
    {
        // Not implemented in Pusher
        return Task.CompletedTask;
    }
}
