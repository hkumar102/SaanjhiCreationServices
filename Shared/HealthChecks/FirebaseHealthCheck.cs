using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using FirebaseAdmin;

namespace Shared.HealthChecks;

public class FirebaseHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var app = FirebaseApp.DefaultInstance;
            return Task.FromResult(app != null
                ? HealthCheckResult.Healthy("Firebase is initialized")
                : HealthCheckResult.Unhealthy("Firebase is not initialized"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Firebase error: {ex.Message}"));
        }
    }
}
