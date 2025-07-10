using RentalService.Contracts.DTOs;
using System.Net.Http.Json;

namespace RentalService.Infrastructure.HttpClients;

public interface ICustomerApiClient
{
    Task<RentalCustomerDto?> GetCustomerByIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<RentalCustomerAddressDto?> GetAddressByIdAsync(Guid addressId, CancellationToken cancellationToken = default);
    Task<List<RentalCustomerDto>> GetCustomersByIdsAsync(List<Guid> customerIds, CancellationToken cancellationToken = default);
}


public class CustomerApiClient(HttpClient httpClient) : ICustomerApiClient
{
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
}