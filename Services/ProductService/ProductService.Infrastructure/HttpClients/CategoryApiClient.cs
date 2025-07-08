using System.Net.Http.Json;
using CategoryService.Contracts.DTOs;

namespace ProductService.Infrastructure.HttpClients;

public class CategoryApiClient(HttpClient httpClient)
{
    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId)
    {
        var response = await httpClient.GetAsync($"/api/Category/{categoryId}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CategoryDto>();
    }
    
    public async Task<List<CategoryDto>?> GetCategoryByIdsAsync(List<Guid> ids)
    {
        var response = await httpClient.PostAsJsonAsync($"/api/Category/by-ids",ids);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
    }
}