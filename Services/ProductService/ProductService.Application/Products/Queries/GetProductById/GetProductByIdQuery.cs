using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public Guid Id { get; set; }
    
    // Include Options
    public bool IncludeMedia { get; set; } = true;
    public bool IncludeInventory { get; set; } = true;
    public bool OrganizeMediaByColor { get; set; } = true;
}
