// File: Application/Categories/Queries/GetCategoriesByIds/GetCategoriesByIdsQueryHandler.cs
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Contracts.DTOs;

namespace CategoryService.Application.Categories.Queries.GetCategoriesByIds;

public class GetCategoriesByIdsQueryHandler(CategoryDbContext context, IMapper mapper)
    : IRequestHandler<GetCategoriesByIdsQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesByIdsQuery request, CancellationToken cancellationToken)
    {
        var categories = await context.Categories
            .Where(c => request.CategoryIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        return mapper.Map<List<CategoryDto>>(categories);
    }
}