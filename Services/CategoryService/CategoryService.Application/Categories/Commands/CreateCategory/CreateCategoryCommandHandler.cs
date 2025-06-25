using AutoMapper;
using MediatR;
using CategoryService.Domain.Entities;
using CategoryService.Infrastructure.Persistence;

namespace CategoryService.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly CategoryDbContext _db;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(CategoryDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _mapper.Map<Category>(request);
        category.Id = Guid.NewGuid();
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
