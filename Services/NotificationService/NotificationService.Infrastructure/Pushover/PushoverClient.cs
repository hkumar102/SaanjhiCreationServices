using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Domain;

namespace NotificationService.Infrastructure.Pushover;

public class PushoverClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PushoverClient> _logger;
    private readonly PushoverOptions _options;

    public PushoverClient(HttpClient httpClient, ILogger<PushoverClient> logger, IOptions<PushoverOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<bool> SendMessageAsync(Notification notification, CancellationToken cancellationToken)
    {
        var apiToken = _options.ApiToken;
        var userKey = _options.UserKey;
        var baseUrl = _options.BaseUrl ?? "https://api.pushover.net/1/messages.json";
        var sb = new StringBuilder();
        sb.Append($"token={apiToken}&user={userKey}");
        sb.Append($"&title={Uri.EscapeDataString(notification.Title)}");
        sb.Append($"&message={Uri.EscapeDataString(notification.Message)}");
        sb.Append($"&url_title=View Details");
        if (!string.IsNullOrWhiteSpace(notification.Metadata))
        {
            try
            {
                var meta = JsonDocument.Parse(notification.Metadata).RootElement;
                if (meta.TryGetProperty("Link", out var linkProp))
                    sb.Append($"&url={Uri.EscapeDataString(linkProp.GetString())}");
            }
            catch { /* ignore metadata parse errors for url fields */ }
        }
        var urlContent = sb.ToString();
        _logger.LogInformation("[Push] Notification sent to user {RecipientUserId} - {apiToken} - {baseUrl} - {urlContent}", userKey, apiToken, baseUrl, urlContent);
        var content = new StringContent(urlContent, Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = await _httpClient.PostAsync(baseUrl, content, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Pushover notification sent successfully.");
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to send Pushover notification: {Error}", error);
            return false;
        }
    }
}
