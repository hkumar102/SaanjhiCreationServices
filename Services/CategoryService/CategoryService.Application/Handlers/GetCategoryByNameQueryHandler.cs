using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Categories;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Application.Queries;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process GetCategoryByNameQuery.
/// </summary>
public class GetCategoryByNameQueryHandler : IRequestHandler<GetCategoryByNameQuery, CategoryDto>
{
    private readonly CategoryDbContext _dbContext;

    public GetCategoryByNameQueryHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto> Handle(GetCategoryByNameQuery request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .Where(c => c.Name == request.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .FirstOrDefaultAsync(cancellationToken);

        return category ?? throw new KeyNotFoundException("Category not found");
    }
}
