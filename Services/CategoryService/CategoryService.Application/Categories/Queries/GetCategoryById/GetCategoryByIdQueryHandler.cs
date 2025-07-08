using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Contracts.DTOs;

namespace CategoryService.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler(CategoryDbContext db, IMapper mapper)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found");

        return mapper.Map<CategoryDto>(category);
    }
}
