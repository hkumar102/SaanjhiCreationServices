using AutoMapper;
using MediatR;
using CategoryService.Domain.Entities;
using CategoryService.Infrastructure.Persistence;

namespace CategoryService.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(CategoryDbContext db, IMapper mapper)
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = mapper.Map<Category>(request);
        category.Id = Guid.NewGuid();
        db.Categories.Add(category);
        await db.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
