using ProductService.Contracts.DTOs;

namespace ProductService.Contracts.Extensions;

/// <summary>
/// Extension methods for organizing and filtering product media
/// </summary>
public static class ProductMediaExtensions
{
    /// <summary>
    /// Get media for a specific color, including generic media
    /// </summary>
    public static IEnumerable<ProductMediaDto> GetMediaForColor(this IEnumerable<ProductMediaDto> media, string color)
    {
        return media.Where(m => m.Color == color || m.IsGeneric)
                   .OrderBy(m => m.SortOrder)
                   .ThenBy(m => m.MediaPurpose);
    }

    /// <summary>
    /// Get main image for a specific color
    /// </summary>
    public static ProductMediaDto? GetMainImageForColor(this IEnumerable<ProductMediaDto> media, string color)
    {
        return media.FirstOrDefault(m => m.Color == color && m.MediaPurpose == "main")
               ?? media.FirstOrDefault(m => m.IsGeneric && m.MediaPurpose == "main")
               ?? media.FirstOrDefault(m => m.Color == color)
               ?? media.FirstOrDefault(m => m.IsGeneric);
    }

    /// <summary>
    /// Get media by purpose (main, detail, back, etc.)
    /// </summary>
    public static IEnumerable<ProductMediaDto> GetMediaByPurpose(this IEnumerable<ProductMediaDto> media, string purpose)
    {
        return media.Where(m => m.MediaPurpose == purpose)
                   .OrderBy(m => m.SortOrder);
    }

    /// <summary>
    /// Get all unique colors that have media
    /// </summary>
    public static IEnumerable<string> GetColorsWithMedia(this IEnumerable<ProductMediaDto> media)
    {
        return media.Where(m => !string.IsNullOrEmpty(m.Color) && !m.IsGeneric)
                   .Select(m => m.Color!)
                   .Distinct()
                   .OrderBy(c => c);
    }

    /// <summary>
    /// Organize media by color
    /// </summary>
    public static Dictionary<string, List<ProductMediaDto>> OrganizeByColor(this IEnumerable<ProductMediaDto> media)
    {
        return media.Where(m => !string.IsNullOrEmpty(m.Color) && !m.IsGeneric)
                   .GroupBy(m => m.Color!)
                   .ToDictionary(
                       g => g.Key,
                       g => g.OrderBy(m => m.SortOrder).ToList()
                   );
    }

    /// <summary>
    /// Organize media by purpose
    /// </summary>
    public static Dictionary<string, List<ProductMediaDto>> OrganizeByPurpose(this IEnumerable<ProductMediaDto> media)
    {
        return media.Where(m => !string.IsNullOrEmpty(m.MediaPurpose))
                   .GroupBy(m => m.MediaPurpose!)
                   .ToDictionary(
                       g => g.Key,
                       g => g.OrderBy(m => m.SortOrder).ToList()
                   );
    }

    /// <summary>
    /// Get generic media (not tied to specific color/size)
    /// </summary>
    public static IEnumerable<ProductMediaDto> GetGenericMedia(this IEnumerable<ProductMediaDto> media)
    {
        return media.Where(m => m.IsGeneric)
                   .OrderBy(m => m.SortOrder);
    }

    /// <summary>
    /// Create a comprehensive media collection DTO
    /// </summary>
    public static ProductMediaCollectionDto ToMediaCollection(this IEnumerable<ProductMediaDto> media, Guid productId, string productName)
    {
        var mediaList = media.ToList();
        
        return new ProductMediaCollectionDto
        {
            ProductId = productId,
            ProductName = productName,
            AllMedia = mediaList,
            MediaByColor = mediaList.OrganizeByColor(),
            MediaByPurpose = mediaList.OrganizeByPurpose(),
            GenericMedia = mediaList.GetGenericMedia().ToList(),
            ColorsWithMedia = mediaList.GetColorsWithMedia().ToList(),
            MainImage = mediaList.GetMainImageForColor("") ?? mediaList.FirstOrDefault(),
            MainImageByColor = mediaList.GetColorsWithMedia()
                .ToDictionary(color => color, color => mediaList.GetMainImageForColor(color))
        };
    }

    /// <summary>
    /// Get fallback image chain for a color
    /// </summary>
    public static ProductMediaDto? GetImageWithFallback(this IEnumerable<ProductMediaDto> media, string? color = null, string? purpose = null)
    {
        var mediaList = media.ToList();

        // Try specific color and purpose
        if (!string.IsNullOrEmpty(color) && !string.IsNullOrEmpty(purpose))
        {
            var specific = mediaList.FirstOrDefault(m => m.Color == color && m.MediaPurpose == purpose);
            if (specific != null) return specific;
        }

        // Try specific color, any purpose
        if (!string.IsNullOrEmpty(color))
        {
            var colorSpecific = mediaList.FirstOrDefault(m => m.Color == color);
            if (colorSpecific != null) return colorSpecific;
        }

        // Try generic with specific purpose
        if (!string.IsNullOrEmpty(purpose))
        {
            var genericPurpose = mediaList.FirstOrDefault(m => m.IsGeneric && m.MediaPurpose == purpose);
            if (genericPurpose != null) return genericPurpose;
        }

        // Try any generic
        var generic = mediaList.FirstOrDefault(m => m.IsGeneric);
        if (generic != null) return generic;

        // Return any image
        return mediaList.FirstOrDefault();
    }
}
