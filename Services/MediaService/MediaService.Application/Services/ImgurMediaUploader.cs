using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using MediaService.Contracts.Interfaces;
using MediaService.Contracts.DTOs;
using MediaService.Contracts.Enums;
using System.Text.Json;

namespace MediaService.Application.Services
{
    public class ImgurMediaUploader(HttpClient httpClient, IConfiguration config) : IMediaUploader
    {
        public async Task<UploadMediaResult> UploadAsync(IFormFile file, MediaType mediaType, CancellationToken cancellationToken = default)
        {
            var clientId = config["Imgur:ClientId"];
            if (string.IsNullOrWhiteSpace(clientId))
                throw new InvalidOperationException("Imgur Client ID is not configured.");

            using var content = new MultipartFormDataContent();
            await using var fileStream = file.OpenReadStream();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "image", file.FileName); // Imgur uses "image" field even for videos

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", clientId);

            var response = await httpClient.PostAsync("https://api.imgur.com/3/upload", content, cancellationToken);
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
}

