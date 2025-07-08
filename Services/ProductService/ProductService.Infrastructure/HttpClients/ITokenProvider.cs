namespace ProductService.Infrastructure.HttpClients;

public interface ITokenProvider
{
    Task<string> GetTokenAsync();
}