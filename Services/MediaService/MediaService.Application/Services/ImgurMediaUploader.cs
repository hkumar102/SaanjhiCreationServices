using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Shared.Contracts.Media;

namespace MediaService.Application.Services;

using Microsoft.AspNetCore.Http;
using Interfaces;
using DTOs;
using System.Text.Json;

public class ImgurMediaUploader : IMediaUploader
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public ImgurMediaUploader(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<UploadMediaResult> UploadAsync(IFormFile file, MediaType mediaType, CancellationToken cancellationToken = default)
    {
        var clientId = _config["Imgur:ClientId"];
        if (string.IsNullOrWhiteSpace(clientId))
            throw new InvalidOperationException("Imgur Client ID is not configured.");

        using var content = new MultipartFormDataContent();
        await using var fileStream = file.OpenReadStream();
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "image", file.FileName); // Imgur uses "image" field even for videos

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", clientId);

        var response = await _httpClient.PostAsync("https://api.imgur.com/3/upload", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonDocument.Parse(json).RootElement.GetProperty("data");

        return new UploadMediaResult
        {
            Url = result.GetProperty("link").GetString()!,
            PublicId = result.GetProperty("id").GetString(),
            MediaType = mediaType
        };
    }
}

