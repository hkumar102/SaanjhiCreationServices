namespace NotificationService.Infrastructure.Pushover;

public class PushoverOptions
{
    public string ApiToken { get; set; } = string.Empty;
    public string UserKey { get; set; } = string.Empty;
    public string? BaseUrl { get; set; } = "https://api.pushover.net/1/messages.json";
}
