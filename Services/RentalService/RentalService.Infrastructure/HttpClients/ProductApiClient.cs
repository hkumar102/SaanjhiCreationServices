using RentalService.Contracts.DTOs;
using System.Net.Http.Json;

namespace RentalService.Infrastructure.HttpClients;

public interface IProductApiClient
{
    Task<RentalProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
}

public class ProductApiClient(HttpClient httpClient) : IProductApiClient
{
    public async Task<RentalProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<RentalProductDto>($"/api/Product/{productId}", cancellationToken);
    }
}