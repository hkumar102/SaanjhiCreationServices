using RentalService.Contracts.DTOs;
using System.Net.Http.Json;

namespace RentalService.Infrastructure.HttpClients;

public interface ICustomerApiClient
{
    Task<RentalCustomerDto?> GetCustomerByIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<RentalCustomerAddressDto?> GetAddressByIdAsync(Guid addressId, CancellationToken cancellationToken = default);
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
}