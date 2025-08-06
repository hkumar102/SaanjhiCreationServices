using RentalService.Contracts.DTOs;
using RentalService.Domain.Entities;
using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RentalService.Infrastructure.HttpClients;
using Shared.Contracts.Common;

namespace RentalService.Application.Rentals.Queries.GetRentals;

using Infrastructure.Persistence;

public class GetRentalsQueryHandler(
    RentalDbContext dbContext,
    IMapper mapper,
    IProductApiClient productClient,
    ILogger<GetRentalsQueryHandler> logger)
    : IRequestHandler<GetRentalsQuery, PaginatedResult<RentalDto>>
{
    public async Task<PaginatedResult<RentalDto>> Handle(GetRentalsQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetRentalsQuery execution with Page={Page}, PageSize={PageSize}, CustomerIds={CustomerIds}, ProductIds={ProductIds}",
            request.Page, request.PageSize, 
            request.CustomerIds?.Count ?? 0, request.ProductIds?.Count ?? 0);

        try
        {
            var query = dbContext.Rentals
                .AsNoTracking()
                .Include(r => r.Timelines)
                .AsQueryable();

            // Filtering
            if (request.CustomerIds != null && request.CustomerIds.Any())
            {
                logger.LogDebug("Filtering by {CustomerCount} customer IDs", request.CustomerIds.Count);
                query = query.Where(r => request.CustomerIds.Contains(r.CustomerId));
            }

            if (request.ProductIds != null && request.ProductIds.Any())
            {
                logger.LogDebug("Filtering by {ProductCount} product IDs", request.ProductIds.Count);
                query = query.Where(r => request.ProductIds.Contains(r.ProductId));
            }

            if (request.FromDate.HasValue)
            {
                logger.LogDebug("Filtering by FromDate: {FromDate}", request.FromDate.Value.Date);
                query = query.Where(r => (r.ActualStartDate ?? r.StartDate) >= request.FromDate.Value.Date);
            }

            if (request.ToDate.HasValue)
            {
                logger.LogDebug("Filtering by ToDate: {ToDate}", request.ToDate.Value.Date);
                query = query.Where(r => (r.ActualReturnDate ?? r.EndDate) <= request.ToDate.Value.Date);
            }
            
            if(request.BookingFromDate.HasValue)
            {
                logger.LogDebug("Filtering by BookingFromDate: {BookingFromDate}", request.BookingFromDate.Value.Date);
                query = query.Where(r => r.BookingDate >= request.BookingFromDate.Value.Date);
            }

            if (request.BookingToDate.HasValue)
            {
                logger.LogDebug("Filtering by BookingToDate: {BookingToDate}", request.BookingToDate.Value.Date);
                query = query.Where(r => r.BookingDate <= request.BookingToDate.Value.Date);
            }

            if (request.Status.HasValue)
            {
                logger.LogDebug("Filtering by Status: {Status}", request.Status);
                query = query.Where(r => r.Status == request.Status.Value);
            }

            // Sorting (basic dynamic sort)
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                logger.LogDebug("Applying sorting: {SortBy}, Descending={Descending}", request.SortBy, request.Descending);
                query = request.SortBy switch
                {
                    nameof(Rental.StartDate) => request.Descending
                        ? query.OrderByDescending(r => r.StartDate)
                        : query.OrderBy(r => r.StartDate),

                    nameof(Rental.RentalPrice) => request.Descending
                        ? query.OrderByDescending(r => r.RentalPrice)
                        : query.OrderBy(r => r.RentalPrice),

                    _ => query.OrderBy(r => r.ModifiedAt).ThenBy(r => r.CreatedAt) // default fallback
                };
            }
            else
            {
                //we want default sort to modified at then created at
                query = query.OrderBy(r => r.ModifiedAt).ThenBy(r => r.CreatedAt);
            }

            // Count total records before pagination
            logger.LogDebug("Counting total records for pagination");
            var totalCount = await query.CountAsync(cancellationToken);
            logger.LogDebug("Found {TotalCount} total records", totalCount);

            // Apply pagination
            logger.LogDebug("Applying pagination: Skip={Skip}, Take={Take}", 
                (request.Page - 1) * request.PageSize, request.PageSize);
            var pagedRentals = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            logger.LogDebug("Retrieved {RentalCount} rentals for current page", pagedRentals.Count);

            // Extract unique IDs for bulk API calls
            var customerIds = pagedRentals.Select(r => r.CustomerId).Distinct().ToList();
            var productIds = pagedRentals.Select(r => r.ProductId).Distinct().ToList();

            logger.LogDebug("Making bulk API calls for {CustomerCount} customers and {ProductCount} products", 
                customerIds.Count, productIds.Count);

            // Bulk API calls
            var customersTask = productClient.GetCustomersByIdsAsync(customerIds, cancellationToken);
            var productsTask = productClient.GetProductsByIdsAsync(productIds, cancellationToken);

            await Task.WhenAll(customersTask, productsTask);

            var customers = await customersTask;
            var products = await productsTask;

            logger.LogDebug("Received {CustomerCount} customers and {ProductCount} products from API calls", 
                customers.Count, products.Count);

            // Create lookup dictionaries for fast access
            var customerLookup = customers.ToDictionary(c => c.Id, c => c);
            var productLookup = products.ToDictionary(p => p.Id, p => p);
            var inventoryLookup = products
                .SelectMany(p => p.InventoryItems.Select(i => new { i.Id, InventoryItem = i }))
                .ToDictionary(x => x.Id, x => x.InventoryItem);
            
            // Create address lookup from customer addresses
            var addressLookup = customers
                .SelectMany(c => c.Addresses.Select(a => new { AddressId = a.Id, Address = a }))
                .ToDictionary(x => x.AddressId, x => x.Address);

            logger.LogDebug("Created lookups with {AddressCount} addresses", addressLookup.Count);

            var result = new List<RentalDto>();

            foreach (var rental in pagedRentals)
            {
                var dto = mapper.Map<RentalDto>(rental);

                // Use lookup dictionaries instead of API calls
                dto.Customer = customerLookup.GetValueOrDefault(rental.CustomerId);
                dto.Product = productLookup.GetValueOrDefault(rental.ProductId);
                dto.InventoryItem = inventoryLookup.GetValueOrDefault(rental.InventoryItemId);
                if (addressLookup.TryGetValue(rental.ShippingAddressId, out var address))
                {
                    dto.ShippingAddress = address.Address();
                }

                result.Add(dto);
            }

            logger.LogDebug("Successfully processed {ResultCount} rental DTOs", result.Count);

            var paginatedResult = new PaginatedResult<RentalDto>
            {
                Items = result,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            logger.LogDebug("GetRentalsQuery completed successfully. Returning {ItemCount} items, Page {Page}, Total={TotalCount}", 
                paginatedResult.Items.Count, paginatedResult.PageNumber, paginatedResult.TotalCount);

            return paginatedResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetRentalsQuery with Page={Page}, PageSize={PageSize}", 
                request.Page, request.PageSize);
            throw;
        }
    }
}
