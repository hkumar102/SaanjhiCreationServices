using MediatR;
using Microsoft.EntityFrameworkCore;
using CategoryService.Infrastructure.Persistence;
using Shared.Contracts.Categories;
using CategoryService.Application.Commands;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process UpdateCategoryCommand.
/// </summary>
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly CategoryDbContext _dbContext;

    public UpdateCategoryCommandHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync([request.Id], cancellationToken);
        if (category == null)
            throw new KeyNotFoundException("Category not found");

        category.Name = request.Name;
        category.Description = request.Description;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}
