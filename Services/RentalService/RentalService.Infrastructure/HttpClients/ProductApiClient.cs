using RentalService.Contracts.DTOs;
using System.Net.Http.Json;

namespace RentalService.Infrastructure.HttpClients;

public interface IProductApiClient
{
    Task<RentalProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<RentalProductDto>> GetProductsByIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default);
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
}