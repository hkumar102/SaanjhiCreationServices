using AutoMapper;
using CategoryService.Application.Categories.Commands.CreateCategory;
using CategoryService.Domain.Entities;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<CreateCategoryCommand, Category>();
        CreateMap<Category, CategoryDto>();
    }
}
