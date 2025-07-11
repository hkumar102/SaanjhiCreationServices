using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Categories.Queries.GetDeletedCategories;

public record GetDeletedCategoriesQuery : IRequest<List<CategoryDto>>;
