using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Categories;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Application.Queries;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process GetCategoryByIdQuery.
/// </summary>
public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly CategoryDbContext _dbContext;

    public GetCategoryByIdQueryHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .Where(c => c.Id == request.Id)
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
