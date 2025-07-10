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

        // Extract unique IDs for bulk API calls
        var customerIds = pagedRentals.Select(r => r.CustomerId).Distinct().ToList();
        var productIds = pagedRentals.Select(r => r.ProductId).Distinct().ToList();

        // Bulk API calls
        var customersTask = customerClient.GetCustomersByIdsAsync(customerIds, cancellationToken);
        var productsTask = productClient.GetProductsByIdsAsync(productIds, cancellationToken);

        await Task.WhenAll(customersTask, productsTask);

        var customers = await customersTask;
        var products = await productsTask;

        // Create lookup dictionaries for fast access
        var customerLookup = customers.ToDictionary(c => c.Id, c => c);
        var productLookup = products.ToDictionary(p => p.Id, p => p);
        
        // Create address lookup from customer addresses
        var addressLookup = customers
            .SelectMany(c => c.Addresses.Select(a => new { AddressId = a.Id, Address = a }))
            .ToDictionary(x => x.AddressId, x => x.Address);

        var result = new List<RentalDto>();

        foreach (var rental in pagedRentals)
        {
            var dto = mapper.Map<RentalDto>(rental);

            // Use lookup dictionaries instead of API calls
            dto.Customer = customerLookup.GetValueOrDefault(rental.CustomerId);
            dto.Product = productLookup.GetValueOrDefault(rental.ProductId);
            
            if (addressLookup.TryGetValue(rental.ShippingAddressId, out var address))
            {
                dto.ShippingAddress = address.Address();
            }

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