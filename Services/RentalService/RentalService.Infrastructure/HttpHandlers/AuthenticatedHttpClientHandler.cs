using System.Net.Http.Headers;
using RentalService.Infrastructure.HttpClients;

namespace RentalService.Infrastructure.HttpHandlers;

public class AuthenticatedHttpClientHandler(ITokenProvider tokenProvider) : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}