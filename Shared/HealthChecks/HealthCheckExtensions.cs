using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Shared.HealthChecks;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddSaanjhiHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!)
            .AddCheck<FirebaseHealthCheck>("firebase");
        return services;
    }
}
