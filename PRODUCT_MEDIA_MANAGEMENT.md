# Product Media Management with Color/Variant Support

## Overview
This document describes the enhanced ProductMedia system that supports color-specific and variant-specific images, enabling customers to see accurate images for each color variant of a product.

## Key Features

### 1. **Color-Specific Media**
- Each media item can be associated with a specific color
- Generic media applies to all colors
- Automatic fallback system for missing color-specific images

### 2. **Media Organization**
- **MediaPurpose**: Categorizes images (main, detail, back, lifestyle, size_chart, etc.)
- **SortOrder**: Controls display order within each category
- **IsGeneric**: Marks images that apply to all variants

### 3. **Flexible Association**
- Media can be tied to specific colors, sizes, or both
- Generic media for product-wide information (size charts, care instructions)
- Support for different media types (images, videos)

## Database Schema

### Updated ProductMedia Entity
```csharp
public class ProductMedia : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Url { get; set; }
    public string? PublicId { get; set; }
    public MediaType MediaType { get; set; }
    public int SortOrder { get; set; }
    
    // Variant Association
    public string? Color { get; set; }        // "Black", "Red", null for all
    public string? Size { get; set; }         // "XL", null for all sizes
    public bool IsGeneric { get; set; }       // Generic for all variants
    
    // Classification
    public string? MediaPurpose { get; set; } // "main", "detail", "back"
    public string? AltText { get; set; }      // Accessibility
    
    public Product Product { get; set; }
}
```

## Media Purposes

| Purpose | Description | Usage |
|---------|-------------|-------|
| `main` | Primary product image | Product listings, thumbnails |
| `detail` | Close-up detail shots | Product details, zoom views |
| `back` | Back view of product | 360° view, detailed inspection |
| `lifestyle` | Styled/lifestyle photos | Marketing, inspiration |
| `size_chart` | Size guide images | Size selection help |
| `care_instructions` | Care label photos | Product care information |
| `fit_guide` | Fit and styling guide | Styling recommendations |

## Business Logic Examples

### 1. **Getting Images for a Color**
```csharp
// Get all images for "Black" color including generic
var blackImages = product.Media
    .Where(m => m.Color == "Black" || m.IsGeneric)
    .OrderBy(m => m.SortOrder);

// Get main image for "Red" with fallback
var redMainImage = product.Media
    .FirstOrDefault(m => m.Color == "Red" && m.MediaPurpose == "main")
    ?? product.Media.FirstOrDefault(m => m.IsGeneric && m.MediaPurpose == "main");
```

### 2. **Organizing Media Collections**
```csharp
// Group media by color
var mediaByColor = product.Media
    .Where(m => !string.IsNullOrEmpty(m.Color) && !m.IsGeneric)
    .GroupBy(m => m.Color)
    .ToDictionary(g => g.Key, g => g.ToList());

// Get generic media
var genericMedia = product.Media.Where(m => m.IsGeneric).ToList();
```

### 3. **Frontend Display Logic**
```csharp
// Get display images for selected color
public List<ProductMediaDto> GetDisplayImages(string selectedColor)
{
    var colorSpecific = Media.Where(m => m.Color == selectedColor).ToList();
    var generic = Media.Where(m => m.IsGeneric).ToList();
    
    return colorSpecific.Any() ? colorSpecific : generic;
}
```

## API Endpoints (Proposed)

### Product Media Management
- `GET /api/products/{id}/media` - Get all media for product
- `GET /api/products/{id}/media/color/{color}` - Get media for specific color
- `POST /api/products/{id}/media` - Add new media
- `PUT /api/products/media/{mediaId}` - Update media
- `DELETE /api/products/media/{mediaId}` - Delete media
- `POST /api/products/{id}/media/bulk` - Bulk media operations

### Media Organization
- `GET /api/products/{id}/media/organized` - Get organized media collections
- `GET /api/products/{id}/media/colors` - Get available colors with media
- `GET /api/products/{id}/media/main-images` - Get main images for all colors

## DTOs

### Core DTOs
- **ProductMediaDto**: Basic media information
- **CreateProductMediaDto**: For creating new media
- **UpdateProductMediaDto**: For updating existing media
- **ProductMediaCollectionDto**: Organized media collections

### Management DTOs
- **BulkMediaOperationDto**: Bulk add/update/delete operations
- **MediaQueryDto**: Media filtering and search
- **MediaOperationResultDto**: Operation results with validation

### Validation DTOs
- **CreateProductMediaValidationDto**: With validation attributes
- **MediaValidationRuleDto**: Business rules and limits

## Extension Methods

The `ProductMediaExtensions` class provides helper methods:

```csharp
// Get media for specific color
var blackMedia = product.Media.GetMediaForColor("Black");

// Get main image with fallback
var mainImage = product.Media.GetMainImageForColor("Red");

// Organize by color
var organized = product.Media.OrganizeByColor();

// Get colors with media
var colors = product.Media.GetColorsWithMedia();
```

## Frontend Integration

### Angular Component Example
```typescript
export interface ProductDisplay {
  selectedColor: string;
  availableColors: string[];
  currentImages: ProductMediaDto[];
  mainImage: ProductMediaDto;
}

getImagesForColor(color: string): ProductMediaDto[] {
  return this.product.mediaByColor[color] || this.product.genericMedia;
}

getMainImageForColor(color: string): ProductMediaDto {
  const colorImages = this.getImagesForColor(color);
  return colorImages.find(img => img.mediaPurpose === 'main') 
         || colorImages[0] 
         || this.product.mainImage;
}
```

### React Hook Example
```typescript
const useProductImages = (product: ProductDto, selectedColor: string) => {
  const images = useMemo(() => {
    return product.mediaByColor[selectedColor] || product.genericMedia;
  }, [product, selectedColor]);

  const mainImage = useMemo(() => {
    return images.find(img => img.mediaPurpose === 'main') || images[0];
  }, [images]);

  return { images, mainImage };
};
```

## Database Indexes

For optimal performance, the following indexes are created:

```sql
-- Performance indexes
CREATE INDEX IX_ProductMedia_ProductId ON ProductMedia(ProductId);
CREATE INDEX IX_ProductMedia_ProductId_Color ON ProductMedia(ProductId, Color);
CREATE INDEX IX_ProductMedia_ProductId_MediaPurpose ON ProductMedia(ProductId, MediaPurpose);
CREATE INDEX IX_ProductMedia_ProductId_IsGeneric ON ProductMedia(ProductId, IsGeneric);
CREATE INDEX IX_ProductMedia_ProductId_Color_MediaPurpose ON ProductMedia(ProductId, Color, MediaPurpose);
```

## Migration Strategy

### Phase 1: Database Update
1. Add new columns to ProductMedia table
2. Set default values for existing records
3. Update indexes

### Phase 2: Application Update
1. Update entity and DTOs
2. Update service layer logic
3. Create new API endpoints

### Phase 3: Frontend Update
1. Update product display components
2. Add color-specific image selection
3. Implement fallback logic

### Phase 4: Data Population
1. Organize existing media by color
2. Set MediaPurpose for existing images
3. Add color-specific images

## Best Practices

### 1. **Image Organization**
- Always provide a main image for each color
- Use consistent naming for MediaPurpose
- Maintain proper SortOrder for display

### 2. **Performance**
- Use appropriate indexes for queries
- Implement lazy loading for media collections
- Cache organized media collections

### 3. **User Experience**
- Provide fallback images when color-specific images are missing
- Show loading states during image transitions
- Implement progressive image loading

### 4. **Content Management**
- Validate image requirements before publishing products
- Provide bulk upload tools for media management
- Implement image optimization and CDN integration

## Validation Rules

### Business Rules
- Maximum 20 images per product
- Maximum 10 images per color variant
- At least one main image per available color
- Required alt text for accessibility
- Supported file formats: JPG, PNG, WebP, GIF
- Maximum file size: 5MB per image

### Data Validation
- Color values must match product's AvailableColors
- Size values must match product's AvailableSizes
- MediaPurpose must be from predefined list
- URLs must be valid and accessible
- Alt text required for accessibility compliance

This enhanced media management system provides the flexibility needed for a modern e-commerce clothing rental platform while maintaining performance and usability.
