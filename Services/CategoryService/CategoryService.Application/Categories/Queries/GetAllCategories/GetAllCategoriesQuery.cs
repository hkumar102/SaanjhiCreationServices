using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<List<CategoryDto>> { }
