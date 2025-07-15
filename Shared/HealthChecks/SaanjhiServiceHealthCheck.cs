using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Shared.HealthChecks;

public class SaanjhiServiceHealthCheck(IHttpClientFactory httpClientFactory, string serviceName, string endpoint)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = endpoint;
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);

        try
        {
            var response = await client.GetAsync("health", cancellationToken);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
                return HealthCheckResult.Healthy($"{serviceName} is healthy");
            return HealthCheckResult.Unhealthy(
                $"{serviceName} is unhealthy: " + await response.Content.ReadAsStringAsync(cancellationToken)
            );
        }
        catch
        {
            throw;
        }
    }
}