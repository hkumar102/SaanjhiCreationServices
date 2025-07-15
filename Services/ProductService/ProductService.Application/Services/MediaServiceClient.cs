using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProductService.Application.Services;

public class MediaServiceClient : IMediaServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MediaServiceClient> _logger;

    public MediaServiceClient(HttpClient httpClient, ILogger<MediaServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<MediaUploadResponse> UploadProductImageAsync(IFormFile file, bool isPrimary = false, string? color = null, string? altText = null)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            
            // Add file
            using var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);
            
            // Build query string
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (isPrimary)
                query["isPrimary"] = isPrimary.ToString();
            if (!string.IsNullOrEmpty(color))
                query["color"] = color;
            if (!string.IsNullOrEmpty(altText))
                query["altText"] = altText;

            var url = $"{_httpClient.BaseAddress}api/media/product/upload";
            var uriBuilder = new UriBuilder(url) { Query = query.ToString() };

            _logger.LogDebug("Uploading file {FileName} to MediaService", file.FileName);

            var response = await _httpClient.PostAsync(uriBuilder.Uri, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"MediaService upload failed: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var uploadResponse = JsonSerializer.Deserialize<MediaUploadResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (uploadResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize MediaService response");
            }

            _logger.LogDebug("Successfully uploaded file {FileName} to MediaService. MediaId: {MediaId}", 
                file.FileName, uploadResponse.MediaId);

            return uploadResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} to MediaService", file.FileName);
            throw;
        }
    }
}
