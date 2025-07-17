using Microsoft.Extensions.Configuration;

namespace MediaService.Application.Services;

/// <summary>
/// Enhanced ImageKit service that provides URL transformations for different sizes
/// </summary>
public class ImageKitUrlService
{
    private readonly string _urlEndpoint;

    public ImageKitUrlService(IConfiguration configuration)
    {
        _urlEndpoint = configuration["ImageKit:UrlEndpoint"] ?? "";
    }

    /// <summary>
    /// Generate optimized image URLs with ImageKit transformations
    /// </summary>
    public string GetOptimizedUrl(string originalUrl, int? width = null, int? height = null, string quality = "85", string format = "auto")
    {
        if (string.IsNullOrEmpty(originalUrl) || !originalUrl.Contains(_urlEndpoint))
        {
            return originalUrl;
        }

        var transformations = new List<string>();
        
        if (width.HasValue) transformations.Add($"w-{width}");
        if (height.HasValue) transformations.Add($"h-{height}");
        
        transformations.Add($"q-{quality}");
        transformations.Add($"f-{format}"); // auto format selection
        transformations.Add("pr-true"); // progressive loading

        var transformationString = string.Join(",", transformations);
        
        // Extract just the file path from the original URL
        // originalUrl: https://ik.imagekit.io/pq3akxlqp/products/file.jpg
        // We need: https://ik.imagekit.io/pq3akxlqp/tr:w-400,h-400,q-85,f-auto/products/file.jpg
        
        var uri = new Uri(originalUrl);
        var pathWithoutLeadingSlash = uri.AbsolutePath.TrimStart('/');
        
        // Remove the imagekit ID from the path if it's duplicated
        var imagekitId = _urlEndpoint.Split('/').Last();
        if (pathWithoutLeadingSlash.StartsWith(imagekitId + "/"))
        {
            pathWithoutLeadingSlash = pathWithoutLeadingSlash.Substring(imagekitId.Length + 1);
        }
        
        return $"{_urlEndpoint}/tr:{transformationString}/{pathWithoutLeadingSlash}";
    }

    /// <summary>
    /// Get thumbnail URL (150x150)
    /// </summary>
    public string GetThumbnailUrl(string originalUrl) 
        => GetOptimizedUrl(originalUrl, 150, null, "80", "auto");

    /// <summary>
    /// Get small image URL (400x400)
    /// </summary>
    public string GetSmallUrl(string originalUrl) 
        => GetOptimizedUrl(originalUrl, 400, null, "85", "auto");

    /// <summary>
    /// Get medium image URL (800x800)
    /// </summary>
    public string GetMediumUrl(string originalUrl) 
        => GetOptimizedUrl(originalUrl, 800, null, "85", "auto");

    /// <summary>
    /// Get large image URL (1200x1200)
    /// </summary>
    public string GetLargeUrl(string originalUrl) 
        => GetOptimizedUrl(originalUrl, 1200, null, "90", "auto");
}
