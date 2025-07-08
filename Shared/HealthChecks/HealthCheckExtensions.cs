using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.HealthChecks;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddSaanjhiHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!)
            .AddCheck<FirebaseHealthCheck>("firebase");
    }
    
    public static IHealthChecksBuilder AddSaanjhiServiceHealthCheck(this IHealthChecksBuilder builder, string serviceName, string endpoint)
    {
        
        return builder
            .AddCheck(serviceName, 
                new SaanjhiServiceHealthCheck(builder.Services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>(), serviceName, endpoint));
    }
}
