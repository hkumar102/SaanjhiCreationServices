using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MediaService.Contracts.Interfaces;

namespace MediaService.Infrastructure.Services;

/// <summary>
/// Local file storage implementation for development
/// In production, this should be replaced with cloud storage (Azure Blob, AWS S3, etc.)
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly string _storagePath;
    private readonly string _baseUrl;

    public LocalFileStorageService(ILogger<LocalFileStorageService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _storagePath = configuration["FileStorage:LocalPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        _baseUrl = configuration["FileStorage:BaseUrl"] ?? "/uploads";
        
        // Convert relative path to absolute if needed
        if (!Path.IsPathRooted(_storagePath))
        {
            _storagePath = Path.Combine(Directory.GetCurrentDirectory(), _storagePath);
        }
        
        // Ensure directory exists
        Directory.CreateDirectory(_storagePath);
        
        _logger.LogInformation("LocalFileStorageService initialized. Storage path: {StoragePath}, Base URL: {BaseUrl}", 
            _storagePath, _baseUrl);
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            // Create subdirectory based on content type
            var subDirectory = contentType.StartsWith("image/") ? "products" : "media";
            var fullStoragePath = Path.Combine(_storagePath, subDirectory);
            Directory.CreateDirectory(fullStoragePath);

            // Generate unique filename to avoid conflicts
            var fileExtension = Path.GetExtension(fileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var uniqueFileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(fullStoragePath, uniqueFileName);

            // Save file to disk
            using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(fileStreamOutput);

            // Return URL path that matches the static file configuration
            var relativePath = Path.Combine(subDirectory, uniqueFileName).Replace("\\", "/");
            var url = $"{_baseUrl.TrimEnd('/')}/{relativePath}";
            
            _logger.LogDebug("Saved file {FileName} to {FilePath}, accessible at {Url}", 
                fileName, filePath, url);

            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file {FileName}", fileName);
            throw;
        }
    }

    public async Task DeleteFileAsync(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_storagePath, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
                _logger.LogDebug("Deleted file {FileName}", fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> GetFileAsync(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_storagePath, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File {fileName} not found");
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return await Task.FromResult(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file {FileName}", fileName);
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(string fileName)
    {
        try
        {
            // Handle subdirectory structure
            var possiblePaths = new[]
            {
                Path.Combine(_storagePath, fileName),
                Path.Combine(_storagePath, "products", fileName),
                Path.Combine(_storagePath, "media", fileName)
            };

            return await Task.FromResult(possiblePaths.Any(File.Exists));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if file exists {FileName}", fileName);
            return false;
        }
    }
}
