using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.HttpClients;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Queries.GetProductsByIds;

public class GetProductsByIdsQueryHandler(ProductDbContext db, IMapper mapper, CategoryApiClient categoryApiClient)
    : IRequestHandler<GetProductsByIdsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsByIdsQuery request, CancellationToken cancellationToken)
    {
        if (!request.ProductIds.Any())
            return new List<ProductDto>();

        var products = await db.Products
            .Include(p => p.Media)
            .Where(p => request.ProductIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var productDtos = mapper.Map<List<ProductDto>>(products);

        // Fetch categories for all products
        var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await categoryApiClient.GetCategoryByIdsAsync(categoryIds) ?? [];

        // Set category names
        foreach (var productDto in productDtos)
        {
            var product = products.FirstOrDefault(p => p.Id == productDto.Id);
            if (product != null)
            {
                var categoryName = categories.FirstOrDefault(c => c.Id == product.CategoryId)?.Name;
                productDto.CategoryName = categoryName;
            }
        }

        return productDtos;
    }
}
