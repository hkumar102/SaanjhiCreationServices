using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.Extensions.Telemetry;

public static class TelemetryExtensions
{
    public static IServiceCollection AddSharedTelemetry(this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        services.AddOpenTelemetry()
            .WithTracing(b =>
            {
                b
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        return services;
    }

    public static WebApplicationBuilder UseSharedSentry(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseSentry(o =>
        {
            o.Dsn = builder.Configuration["Sentry:Dsn"];
            o.Debug = true;
            o.TracesSampleRate = 1.0;
            o.SendDefaultPii = true;
            o.Environment = builder.Environment.EnvironmentName;
            o.AttachStacktrace = true;
            // Configure log levels
            o.MinimumBreadcrumbLevel = LogLevel.Information; // Capture logs as breadcrumbs starting from Information level
            o.MinimumEventLevel = LogLevel.Warning;
        });

        builder.Logging.AddSentry();

        return builder;
    }
}
