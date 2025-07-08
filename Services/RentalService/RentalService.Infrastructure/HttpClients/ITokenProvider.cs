namespace RentalService.Infrastructure.HttpClients;

public interface ITokenProvider
{
    Task<string> GetTokenAsync();
}