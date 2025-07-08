using MediatR;
using Microsoft.EntityFrameworkCore;
using CategoryService.Infrastructure.Persistence;

namespace CategoryService.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(CategoryDbContext db) : IRequestHandler<UpdateCategoryCommand>
{
    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found");

        category.Name = request.Name;
        category.Description = request.Description;

        await db.SaveChangesAsync(cancellationToken);
    }
}
