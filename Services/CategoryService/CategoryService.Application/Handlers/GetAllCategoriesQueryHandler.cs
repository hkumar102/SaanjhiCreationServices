using MediatR;
using Microsoft.EntityFrameworkCore;
using CategoryService.Infrastructure.Persistence;
using Shared.Contracts.Categories;
using CategoryService.Application.Queries;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process GetAllCategoriesQuery.
/// </summary>
public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly CategoryDbContext _dbContext;

    public GetAllCategoriesQueryHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToListAsync(cancellationToken);
    }
}
