using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NotificationService.Application;

public class SmsNotificationService : INotificationService
{
    private readonly ILogger<SmsNotificationService> _logger;

    public SmsNotificationService(ILogger<SmsNotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyAdminAsync(string eventName, object payload)
    {
        // Not implemented in SMS
        return Task.CompletedTask;
    }

    public Task NotifyCustomerSmsAsync(string phoneNumber, string message)
    {
        // TODO: Integrate with SMS provider (e.g., Twilio)
        _logger.LogInformation($"SMS sent to {phoneNumber}: {message}");
        return Task.CompletedTask;
    }
}
