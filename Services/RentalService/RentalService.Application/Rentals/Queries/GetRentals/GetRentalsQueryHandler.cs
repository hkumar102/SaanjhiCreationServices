using RentalService.Contracts.DTOs;
using RentalService.Domain.Entities;
using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentalService.Infrastructure.HttpClients;
using Shared.Contracts.Common;

namespace RentalService.Application.Rentals.Queries.GetRentals;

using Infrastructure.Persistence;

public class GetRentalsQueryHandler(
    RentalDbContext dbContext,
    IMapper mapper,
    ICustomerApiClient customerClient,
    IProductApiClient productClient)
    : IRequestHandler<GetRentalsQuery, PaginatedResult<RentalDto>>
{
    public async Task<PaginatedResult<RentalDto>> Handle(GetRentalsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Rentals.AsNoTracking().AsQueryable();

        // Filtering
        if (request.CustomerId.HasValue)
            query = query.Where(r => r.CustomerId == request.CustomerId);

        if (request.ProductId.HasValue)
            query = query.Where(r => r.ProductId == request.ProductId);

        if (request.FromDate.HasValue)
            query = query.Where(r => r.StartDate >= request.FromDate);

        if (request.ToDate.HasValue)
            query = query.Where(r => r.EndDate <= request.ToDate);

        // Sorting (basic dynamic sort)
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            query = request.SortBy switch
            {
                nameof(Rental.StartDate) => request.Descending
                    ? query.OrderByDescending(r => r.StartDate)
                    : query.OrderBy(r => r.StartDate),

                nameof(Rental.RentalPrice) => request.Descending
                    ? query.OrderByDescending(r => r.RentalPrice)
                    : query.OrderBy(r => r.RentalPrice),

                _ => query.OrderBy(r => r.Id) // default fallback
            };
        }
        else
        {
            query = query.OrderBy(r => r.Id);
        }

        // Count total records before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var pagedRentals = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var rentals = await query.ToListAsync(cancellationToken);
        var result = new List<RentalDto>();

        foreach (var rental in rentals)
        {
            var dto = mapper.Map<RentalDto>(rental);

            dto.Customer = await customerClient.GetCustomerByIdAsync(rental.CustomerId, cancellationToken);
            var customerAddress =
                await customerClient.GetAddressByIdAsync(rental.ShippingAddressId, cancellationToken);
            dto.ShippingAddress = customerAddress?.Address();
            dto.Product = await productClient.GetProductByIdAsync(rental.ProductId, cancellationToken);
            result.Add(dto);
        }

        return new PaginatedResult<RentalDto>
        {
            Items = result,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}