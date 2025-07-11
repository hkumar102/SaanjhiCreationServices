using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Contracts.DTOs;

namespace CategoryService.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler(
    CategoryDbContext db, 
    IMapper mapper,
    ILogger<GetCategoryByIdQueryHandler> logger)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetCategoryByIdQuery execution for CategoryId: {CategoryId}", request.Id);

        try
        {
            logger.LogDebug("Fetching category with ID: {CategoryId} from database", request.Id);
            var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found");

            logger.LogDebug("Found category: {CategoryName}", category.Name);

            var result = mapper.Map<CategoryDto>(category);
            
            logger.LogDebug("GetCategoryByIdQuery completed successfully for CategoryId: {CategoryId}", request.Id);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetCategoryByIdQuery for CategoryId: {CategoryId}", request.Id);
            throw;
        }
    }
}
