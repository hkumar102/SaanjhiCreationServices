using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Notifications.Factories;
using NotificationService.Contracts;
using NotificationService.Contracts.DTOs;
using NotificationService.Infrastructure.Pushover;

namespace NotificationService.Application.Extensions;

public static class NotificationServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind Pushover options from configuration
        services.Configure<PushoverOptions>(configuration.GetSection("Pushover"));
        var notificationConfig = new NotificationConfig();
        configuration.GetSection("NotificationSettings").Bind(notificationConfig);
        services.AddSingleton(notificationConfig);

        services.AddHttpClient<PushoverClient>(c =>
        {
            var baseUrl = configuration["Pushover:BaseUrl"];
            if (baseUrl != null) c.BaseAddress = new Uri(baseUrl);
        });
        // 
        // Register PushNotificationSender
        services.AddSingleton<PushNotificationSender>();
        // Register NotificationSenderFactory
        services.AddSingleton<NotificationSenderFactory>();
        // Register INotificationSender for Push (can be extended for SMS/Email)
        services.AddSingleton<INotificationSender, PushNotificationSender>();
        
        var assemblyName = "NotificationService.Application";
        var assembly = Assembly.Load(assemblyName);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(assembly);
        services.AddAutoMapper(assembly);
        return services;
    }
}
