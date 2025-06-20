using MediatR;
using CategoryService.Domain.Entities;
using CategoryService.Infrastructure.Persistence;
using Shared.Contracts.Categories;
using CategoryService.Application.Commands;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process CreateCategoryCommand.
/// </summary>
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly CategoryDbContext _dbContext;

    public CreateCategoryCommandHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}
