using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Queries;

/// <summary>
/// Query to get all categories.
/// </summary>
public class GetAllCategoriesQuery : IRequest<List<CategoryDto>> { }
