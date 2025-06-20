using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using FluentValidation;
using Shared.Authentication;

namespace Shared.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers common application services like MediatR, FluentValidation, Swagger, and FirebaseAuth.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, Assembly applicationAssembly)
    {
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        // FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(applicationAssembly);
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = FirebaseAuthenticationDefaults.Scheme;
            options.DefaultChallengeScheme = FirebaseAuthenticationDefaults.Scheme;
        }).AddFirebaseAuthentication();
        
        services.AddAuthorization();
        // Controllers
        services.AddControllers();

        return services;
    }

    /// <summary>
    /// Adds and configures Swagger.
    /// </summary>
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services, string title)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = "v1" });
            
            // Firebase JWT Bearer config (if enabled later)
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization using Firebase Bearer token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme { Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }}, new List<string>()
                }
            });
        });

        return services;
    }
}
