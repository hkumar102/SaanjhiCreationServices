using MediatR;
using CategoryService.Infrastructure.Persistence;

namespace CategoryService.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(CategoryDbContext db) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await db.Categories.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found");

        db.Categories.Remove(category);
        await db.SaveChangesAsync(cancellationToken);
    }
}
