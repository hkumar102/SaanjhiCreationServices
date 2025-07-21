using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Shared.Authentication;
using AutoMapper;

namespace Shared.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers MediatR and FluentValidation using the provided assembly name.
    /// </summary>
    public static IServiceCollection RegisterAssemblyServices(this IServiceCollection services, string assemblyName)
    {
        var assembly = Assembly.Load(assemblyName);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(assembly);
        services.AddAutoMapper(assembly);
        return services;
    }
    /// <summary>
    /// Registers common application services like MediatR, FluentValidation, Swagger, and FirebaseAuth.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUserService, FirebaseCurrentUserService>();

        //Firebase Authentication
        var firebaseConfig = configuration["FirebaseSecretPath"];
        FirebaseInitializer.InitializeFirebase(firebaseConfig);
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = FirebaseAuthenticationDefaults.Scheme;
            options.DefaultChallengeScheme = FirebaseAuthenticationDefaults.Scheme;
        })
            .AddFirebaseAuthentication();
        
        services.AddAuthorization();
        // Controllers
        services.AddControllers();
        
        
        // Add CORS policy
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        services.AddHttpClient();
        
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
