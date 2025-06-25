using MediatR;
using Microsoft.EntityFrameworkCore;
using CategoryService.Infrastructure.Persistence;

namespace CategoryService.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly CategoryDbContext _db;

    public UpdateCategoryCommandHandler(CategoryDbContext db)
    {
        _db = db;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found");

        category.Name = request.Name;
        category.Description = request.Description;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
