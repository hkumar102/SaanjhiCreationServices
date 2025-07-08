using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Guid>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsRentable { get; set; } = false;
    public decimal? RentalPrice { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public int? MaxRentalDays { get; set; }
    public Guid CategoryId { get; set; }
    public List<ProductMediaDto> Media { get; set; } = new();
}
