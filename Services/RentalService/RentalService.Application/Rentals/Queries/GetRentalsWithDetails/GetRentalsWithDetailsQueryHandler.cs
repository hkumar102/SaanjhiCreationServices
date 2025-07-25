using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using RentalService.Contracts.DTOs;
using RentalService.Infrastructure.Persistence;
using RentalService.Infrastructure.HttpClients;
using RentalService.Contracts.Enums;

namespace RentalService.Application.Rentals.Queries.GetRentalsWithDetails;

public class GetRentalsWithDetailsQueryHandler : IRequestHandler<GetRentalsWithDetailsQuery, List<RentalDto>>
{
    private readonly RentalDbContext _dbContext;
    private readonly IProductApiClient _productApiClient;
    private readonly IMapper _mapper;

    public GetRentalsWithDetailsQueryHandler(
        RentalDbContext dbContext,
        IProductApiClient productApiClient,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _productApiClient = productApiClient;
        _mapper = mapper;
    }

    public async Task<List<RentalDto>> Handle(GetRentalsWithDetailsQuery request, CancellationToken cancellationToken)
    {
        // Use the provided queryable for flexible filtering
        var rentals = request.Queryable.ToList();

        // Collect all unique IDs for batch API calls
        var productIds = request.IncludeProduct ? rentals.Select(r => r.ProductId).Distinct().ToList() : null;
        var customerIds = request.IncludeCustomer ? rentals.Select(r => r.CustomerId).Distinct().ToList() : null;
        var inventoryIds = request.IncludeInventory ? rentals.Select(r => r.InventoryItemId).Distinct().ToList() : null;

        // Batch API calls
        var productsTask = request.IncludeProduct && productIds != null ? _productApiClient.GetProductsByIdsAsync(productIds, cancellationToken) : Task.FromResult(new List<RentalProductDto>());
        var customersTask = request.IncludeCustomer && customerIds != null ? _productApiClient.GetCustomersByIdsAsync(customerIds, cancellationToken) : Task.FromResult(new List<RentalCustomerDto>());

        await Task.WhenAll(productsTask, customersTask);

        var products = productsTask.Result;
        var customers = customersTask.Result;
        var inventoryLookup = products
                .SelectMany(p => p.InventoryItems.Select(i => new { i.Id, InventoryItem = i }))
                .ToDictionary(x => x.Id, x => x.InventoryItem);

        // Create lookup dictionaries
        var productLookup = products.ToDictionary(p => p.Id, p => p);
        var customerLookup = customers.ToDictionary(c => c.Id, c => c);

        // Map rentals to DTOs and assign related data from lookups
        var result = rentals.Select(rental => {
            var dto = _mapper.Map<RentalDto>(rental);
            if (request.IncludeProduct && productLookup.TryGetValue(rental.ProductId, out var product))
                dto.Product = product;
            if (request.IncludeCustomer && customerLookup.TryGetValue(rental.CustomerId, out var customer))
                dto.Customer = customer;
            if (request.IncludeInventory && inventoryLookup.TryGetValue(rental.InventoryItemId, out var inventory))
                dto.InventoryItem = inventory;
            return dto;
        }).ToList();

        return result;
    }
}
