using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Contracts.DTOs;

namespace CategoryService.Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler(CategoryDbContext db, IMapper mapper)
    : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await db.Categories
            .AsNoTracking()
            .ProjectTo<CategoryDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
