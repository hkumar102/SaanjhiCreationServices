namespace RentalService.Contracts.DTOs;

public class RentalProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public required string CategoryName { get; set; }
    public List<RentalProductMediaDto> Media { get; set; } = new();
    public RentalProductMediaDto? MainImage { get; set; }
    public List<RentalInventoryItemDto> InventoryItems { get; set; } = new();
}

public class RentalProductMediaDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = null!;
}