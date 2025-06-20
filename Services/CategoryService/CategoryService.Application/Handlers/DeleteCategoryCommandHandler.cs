using MediatR;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Application.Commands;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process DeleteCategoryCommand.
/// </summary>
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly CategoryDbContext _dbContext;

    public DeleteCategoryCommandHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync([request.Id], cancellationToken);
        if (category == null)
            throw new KeyNotFoundException("Category not found");

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
