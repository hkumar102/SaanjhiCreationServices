using RentalService.Contracts.DTOs;
using System.Net.Http.Json;

namespace RentalService.Infrastructure.HttpClients;

public interface IProductApiClient
{
    Task<RentalProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<RentalProductDto>> GetProductsByIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default);
    Task<RentalCustomerDto?> GetCustomerByIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<RentalCustomerAddressDto?> GetAddressByIdAsync(Guid addressId, CancellationToken cancellationToken = default);
    Task<List<RentalCustomerDto>> GetCustomersByIdsAsync(List<Guid> customerIds, CancellationToken cancellationToken = default);
    Task<RentalInventoryItemDto> GetInventoryByIdAsync(Guid inventoryId, CancellationToken cancellationToken = default);
    Task UpdateInventoryItemStatusAsync(Guid inventoryId, InventoryStatus status, string? notes, CancellationToken cancellationToken = default);
}

public class ProductApiClient(HttpClient httpClient) : IProductApiClient
{
    public async Task<RentalProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<RentalProductDto>($"/api/Product/{productId}", cancellationToken);
    }

    public async Task<List<RentalProductDto>> GetProductsByIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default)
    {
        var requestBody = new { ProductIds = productIds };
        var response = await httpClient.PostAsJsonAsync("/api/Product/by-ids", requestBody, cancellationToken);
        response.EnsureSuccessStatusCode();

        var products = await response.Content.ReadFromJsonAsync<List<RentalProductDto>>(cancellationToken: cancellationToken);
        return products ?? new List<RentalProductDto>();
    }

    public async Task<RentalCustomerDto?> GetCustomerByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<RentalCustomerDto>($"/api/Customer/{customerId}", cancellationToken);
    }

    public async Task<RentalCustomerAddressDto?> GetAddressByIdAsync(Guid addressId, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<RentalCustomerAddressDto>($"/api/CustomerAddress/{addressId}");
    }

    public async Task<List<RentalCustomerDto>> GetCustomersByIdsAsync(List<Guid> customerIds, CancellationToken cancellationToken = default)
    {
        var requestBody = new { CustomerIds = customerIds };
        var response = await httpClient.PostAsJsonAsync("/api/Customer/by-ids", requestBody, cancellationToken);
        response.EnsureSuccessStatusCode();

        var customers = await response.Content.ReadFromJsonAsync<List<RentalCustomerDto>>(cancellationToken: cancellationToken);
        return customers ?? new List<RentalCustomerDto>();
    }

    public async Task<RentalInventoryItemDto> GetInventoryByIdAsync(Guid inventoryId, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<RentalInventoryItemDto>($"/api/Inventory/{inventoryId}", cancellationToken);
    }

    public async Task UpdateInventoryItemStatusAsync(Guid inventoryId, InventoryStatus status, string? notes, CancellationToken cancellationToken = default)
    {
        var requestBody = new { InventoryItemId = inventoryId, Status = status, Notes = notes };
        var response = await httpClient.PutAsJsonAsync($"/api/Inventory/{inventoryId}/status", requestBody, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}