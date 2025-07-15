using System.ComponentModel.DataAnnotations;

namespace ProductService.Contracts.DTOs;

/// <summary>
/// DTO for creating a new product with validation attributes
/// </summary>
public class CreateProductDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;
    
    [StringLength(2000)]
    public string? Description { get; set; }
    
    [StringLength(100)]
    public string? Brand { get; set; }
    
    [StringLength(100)]
    public string? Designer { get; set; }
    
    [StringLength(50)]
    public string? SKU { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal PurchasePrice { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal RentalPrice { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? SecurityDeposit { get; set; }
    
    [Range(1, 365)]
    public int MaxRentalDays { get; set; } = 7;
    
    public string[] AvailableSizes { get; set; } = Array.Empty<string>();
    public string[] AvailableColors { get; set; } = Array.Empty<string>();
    
    [StringLength(100)]
    public string? Material { get; set; }
    
    [StringLength(500)]
    public string? CareInstructions { get; set; }
    
    [StringLength(100)]
    public string? Occasion { get; set; }
    
    [StringLength(50)]
    public string? Season { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool IsRentable { get; set; } = true;
    public bool IsPurchasable { get; set; } = false;
    
    [Required]
    public Guid CategoryId { get; set; }
}

/// <summary>
/// DTO for updating an existing product with validation attributes
/// </summary>
public class UpdateProductDto
{
    [StringLength(200)]
    public string? Name { get; set; }
    
    [StringLength(2000)]
    public string? Description { get; set; }
    
    [StringLength(100)]
    public string? Brand { get; set; }
    
    [StringLength(100)]
    public string? Designer { get; set; }
    
    [StringLength(50)]
    public string? SKU { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal? PurchasePrice { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal? RentalPrice { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? SecurityDeposit { get; set; }
    
    [Range(1, 365)]
    public int? MaxRentalDays { get; set; }
    
    public string[]? AvailableSizes { get; set; }
    public string[]? AvailableColors { get; set; }
    
    [StringLength(100)]
    public string? Material { get; set; }
    
    [StringLength(500)]
    public string? CareInstructions { get; set; }
    
    [StringLength(100)]
    public string? Occasion { get; set; }
    
    [StringLength(50)]
    public string? Season { get; set; }
    
    public bool? IsActive { get; set; }
    public bool? IsRentable { get; set; }
    public bool? IsPurchasable { get; set; }
    
    public Guid? CategoryId { get; set; }
}

/// <summary>
/// DTO for product creation/update operations result
/// </summary>
public class ProductOperationResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public ProductDto? Product { get; set; }
    public Guid? ProductId { get; set; }
}
