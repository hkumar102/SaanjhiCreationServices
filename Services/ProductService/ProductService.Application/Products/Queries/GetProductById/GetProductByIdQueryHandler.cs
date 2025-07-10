using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.HttpClients;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(ProductDbContext db, IMapper mapper, CategoryApiClient categoryApiClient)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await db.Products
                          .Include(p => p.Media)
                          .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                      ?? throw new KeyNotFoundException("Product not found");
        
        var productDto = mapper.Map<ProductDto>(product);
        // Fetch category from CategoryApiClient
        var category = await categoryApiClient.GetCategoryByIdAsync(product.CategoryId);
        productDto.CategoryName = category?.Name;

        return productDto;
    }
}