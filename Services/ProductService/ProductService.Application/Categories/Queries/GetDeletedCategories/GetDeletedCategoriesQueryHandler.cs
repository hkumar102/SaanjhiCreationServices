using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;
using Shared.Infrastructure.Extensions;

namespace ProductService.Application.Categories.Queries.GetDeletedCategories;

public class GetDeletedCategoriesQueryHandler(
    ProductDbContext db,
    IMapper mapper,
    ILogger<GetDeletedCategoriesQueryHandler> logger)
    : IRequestHandler<GetDeletedCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetDeletedCategoriesQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetDeletedCategoriesQuery execution");

        try
        {
            var deletedCategories = await db.GetSoftDeleted<Domain.Entities.Category>()
                .OrderByDescending(c => c.DeletedAt)
                .ToListAsync(cancellationToken);

            var result = mapper.Map<List<CategoryDto>>(deletedCategories);
            
            logger.LogDebug("Retrieved {Count} deleted categories", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetDeletedCategoriesQuery");
            throw;
        }
    }
}
