#!/bin/bash

echo "📦 Creating Shared HealthChecks structure..."

mkdir -p Shared/HealthChecks

# HealthCheckExtensions.cs
cat <<EOF > Shared/HealthChecks/HealthCheckExtensions.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Shared.HealthChecks;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddSaanjhiHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!);

        // To enable Firebase health check, uncomment the next line:
        // services.AddCheck<FirebaseHealthCheck>("firebase");

        return services;
    }
}
EOF

# FirebaseHealthCheck.cs
cat <<EOF > Shared/HealthChecks/FirebaseHealthCheck.cs
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
EOF

echo "✅ Shared health check setup complete."
