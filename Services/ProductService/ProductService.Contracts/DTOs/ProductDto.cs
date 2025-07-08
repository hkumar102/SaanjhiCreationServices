namespace ProductService.Contracts.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsActive { get; set; }
    public bool IsRentable { get; set; }
    public decimal? RentalPrice { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public int? MaxRentalDays { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public List<ProductMediaDto> Media { get; set; } = new();
}
