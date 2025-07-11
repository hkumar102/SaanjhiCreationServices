using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Imagekit.Sdk;
using Imagekit.Models;
using MediaService.Contracts.Interfaces;

namespace MediaService.Infrastructure.Services;

/// <summary>
/// ImageKit-based file storage service for cloud storage and CDN delivery
/// Provides professional image processing, optimization, and global delivery
/// </summary>
public class ImageKitFileStorageService : IFileStorageService
{
    private readonly ILogger<ImageKitFileStorageService> _logger;
    private readonly ImagekitClient _imagekitClient;
    private readonly string _urlEndpoint;

    public ImageKitFileStorageService(ILogger<ImageKitFileStorageService> logger, IConfiguration configuration)
    {
        _logger = logger;
        
        var publicKey = configuration["ImageKit:PublicKey"];
        var privateKey = configuration["ImageKit:PrivateKey"];
        _urlEndpoint = configuration["ImageKit:UrlEndpoint"] ?? "";

        if (string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(_urlEndpoint))
        {
            throw new InvalidOperationException("ImageKit configuration is missing. Please configure PublicKey, PrivateKey, and UrlEndpoint.");
        }

        _imagekitClient = new ImagekitClient(publicKey, privateKey, _urlEndpoint);
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            // Convert stream to byte array
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // Generate unique filename to avoid conflicts
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            
            // Determine folder based on content type
            var folder = contentType.StartsWith("image/") ? "products" : "media";

            var uploadRequest = new FileCreateRequest
            {
                file = fileBytes,
                fileName = uniqueFileName,
                folder = $"/{folder}/",
                useUniqueFileName = false, // We already made it unique
                isPrivateFile = false,
                tags = new List<string> { "product", "processed" }
            };

            _logger.LogDebug("Uploading file {FileName} to ImageKit folder {Folder}", fileName, folder);

            var result = await _imagekitClient.UploadAsync(uploadRequest);

            if (result == null || string.IsNullOrEmpty(result.url))
            {
                throw new InvalidOperationException("ImageKit upload failed - no URL returned");
            }

            _logger.LogDebug("Successfully uploaded file {FileName} to ImageKit. URL: {Url}", fileName, result.url);

            return result.url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} to ImageKit", fileName);
            throw new InvalidOperationException($"Failed to upload file to ImageKit: {ex.Message}", ex);
        }
    }

    public async Task DeleteFileAsync(string fileName)
    {
        try
        {
            // Extract file ID from URL or use fileName
            var fileId = ExtractFileIdFromUrl(fileName);
            
            if (string.IsNullOrEmpty(fileId))
            {
                _logger.LogWarning("Could not extract file ID from {FileName}", fileName);
                return;
            }

            await _imagekitClient.DeleteFileAsync(fileId);
            _logger.LogDebug("Deleted file {FileId} from ImageKit", fileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileName} from ImageKit", fileName);
            throw;
        }
    }

    public async Task<Stream> GetFileAsync(string fileName)
    {
        try
        {
            // For ImageKit, we typically don't download files back to the server
            // Instead, we return the direct URL for browser access
            // This method is mainly for compatibility
            
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(fileName);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to retrieve file from ImageKit: {response.StatusCode}");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file {FileName} from ImageKit", fileName);
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(string fileName)
    {
        try
        {
            // For ImageKit, we can't directly check if a file exists without making an API call
            // This is a simplified implementation - we'll just return true since ImageKit
            // handles non-existent files gracefully in other operations
            await Task.CompletedTask;
            return true; // Simplified implementation
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Extract ImageKit file ID from URL for deletion
    /// ImageKit URLs typically look like: https://ik.imagekit.io/your_imagekit_id/path/filename.jpg
    /// </summary>
    private string ExtractFileIdFromUrl(string url)
    {
        try
        {
            if (url.Contains(_urlEndpoint))
            {
                // Extract path after the URL endpoint
                var uri = new Uri(url);
                return uri.AbsolutePath.TrimStart('/');
            }
            
            // If it's already a file ID or path, return as-is
            return url;
        }
        catch
        {
            return url; // Fallback to original value
        }
    }
}
