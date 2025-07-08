using AutoMapper;
using CustomerService.Contracts.DTOs;
using MediatR;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Customers.Queries;

public class GetAllCustomersQueryHandler(CustomerDbContext context, IMapper mapper)
    : IRequestHandler<GetAllCustomersQuery, List<CustomerDto>>
{
    public async Task<List<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var query = context.Customers.Include(c => c.Addresses).AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(c => c.Name.Contains(request.Name));
        if (!string.IsNullOrWhiteSpace(request.Email))
            query = query.Where(c => c.Email.Contains(request.Email));
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            query = query.Where(c => c.PhoneNumber != null && c.PhoneNumber.Contains(request.PhoneNumber));

        // Sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = request.SortBy.ToLower() switch
            {
                "name" => request.SortDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "email" => request.SortDesc ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
                "createdat" => request.SortDesc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                _ => query
            };
        }

        // Pagination
        query = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);

        var customers = await query.ToListAsync(cancellationToken);
        return mapper.Map<List<CustomerDto>>(customers);
    }
}