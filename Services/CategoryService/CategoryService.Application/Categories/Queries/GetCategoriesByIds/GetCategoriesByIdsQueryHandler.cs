// File: Application/Categories/Queries/GetCategoriesByIds/GetCategoriesByIdsQueryHandler.cs
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Contracts.DTOs;

namespace CategoryService.Application.Categories.Queries.GetCategoriesByIds;

public class GetCategoriesByIdsQueryHandler(
    CategoryDbContext context, 
    IMapper mapper,
    ILogger<GetCategoriesByIdsQueryHandler> logger)
    : IRequestHandler<GetCategoriesByIdsQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesByIdsQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting {HandlerName} execution with {CategoryCount} category IDs", 
            nameof(GetCategoriesByIdsQueryHandler), request.CategoryIds.Count);

        try
        {
            logger.LogDebug("Fetching categories with IDs: {CategoryIds}", request.CategoryIds);
            var categories = await context.Categories
                .Where(c => request.CategoryIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            logger.LogDebug("Found {CategoryCount} categories", categories.Count);
            var result = mapper.Map<List<CategoryDto>>(categories);

            logger.LogDebug("{HandlerName} completed successfully. Returning {CategoryCount} categories", 
                nameof(GetCategoriesByIdsQueryHandler), result.Count);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing {HandlerName} with category IDs: {CategoryIds}", 
                nameof(GetCategoriesByIdsQueryHandler), request.CategoryIds);
            throw;
        }
    }
}
