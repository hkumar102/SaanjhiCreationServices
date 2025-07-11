using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CategoryService.Domain.Entities;
using CategoryService.Infrastructure.Persistence;

namespace CategoryService.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(
    CategoryDbContext db, 
    IMapper mapper,
    ILogger<CreateCategoryCommandHandler> logger)
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting CreateCategoryCommand execution for category: {CategoryName}", request.Name);

        try
        {
            var category = mapper.Map<Category>(request);
            category.Id = Guid.NewGuid();

            logger.LogDebug("Created category entity with ID: {CategoryId}", category.Id);

            db.Categories.Add(category);
            await db.SaveChangesAsync(cancellationToken);
            
            logger.LogDebug("Successfully created category with ID: {CategoryId}", category.Id);
            
            return category.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating category: {CategoryName}", request.Name);
            throw;
        }
    }
}
