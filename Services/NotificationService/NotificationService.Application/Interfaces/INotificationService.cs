using System.Threading.Tasks;

namespace NotificationService.Application.Interfaces;

public interface INotificationService
{
    Task NotifyAdminAsync(string eventName, object payload);
    Task NotifyCustomerSmsAsync(string phoneNumber, string message);
}
