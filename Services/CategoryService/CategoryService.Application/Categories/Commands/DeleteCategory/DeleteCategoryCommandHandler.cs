using MediatR;
using CategoryService.Infrastructure.Persistence;

namespace CategoryService.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly CategoryDbContext _db;

    public DeleteCategoryCommandHandler(CategoryDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found");

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
