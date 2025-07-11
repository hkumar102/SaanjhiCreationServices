using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Services;

namespace Shared.Infrastructure.Extensions;

/// <summary>
/// Dependency injection extensions for shared services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds shared infrastructure services to the DI container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        // Current user service
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
