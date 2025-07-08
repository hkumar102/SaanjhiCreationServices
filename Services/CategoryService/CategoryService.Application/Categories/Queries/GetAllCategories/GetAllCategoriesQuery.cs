using MediatR;
using CategoryService.Contracts.DTOs;

namespace CategoryService.Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<List<CategoryDto>> { }
