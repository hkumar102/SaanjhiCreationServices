using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Guid>
{
    // Basic Product Information
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Designer { get; set; }
    public string? SKU { get; set; }

    // Pricing Information
    public decimal PurchasePrice { get; set; }
    public decimal RentalPrice { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public int MaxRentalDays { get; set; } = 7;

    // Product Specifications
    public string[] AvailableSizes { get; set; } = Array.Empty<string>();
    public string[] AvailableColors { get; set; } = Array.Empty<string>();
    public string? Material { get; set; }
    public string? CareInstructions { get; set; }
    public string? Occasion { get; set; }
    public string? Season { get; set; }

    // Business Logic
    public bool IsActive { get; set; } = true;
    public bool IsRentable { get; set; } = true;
    public bool IsPurchasable { get; set; } = false;

    // Relationships
    public Guid CategoryId { get; set; }
    public List<ProductMediaDto> Media { get; set; } = new();
}
