using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using RentalService.Infrastructure.HttpClients;
using RentalService.Infrastructure.HttpHandlers;
using Shared.Extensions.Telemetry;
using RentalService.Infrastructure.Persistence;
using Shared.Extensions;
using Shared.HealthChecks;
using Shared.Infrastructure.Extensions;
using Shared.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

builder.UseSharedSentry();
builder.Services.AddSharedTelemetry(builder.Configuration, "RentalService");
// Common shared service registration
var appAssembly = "RentalService.Application";
builder.Services.RegisterAssemblyServices(appAssembly);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddTransient<AuthenticatedHttpClientHandler>();
builder.Services.AddTransient<ITokenProvider, TokenProvider>();

builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>(c =>
    {
        var productServiceUrl = builder.Configuration["HttpClient:ProductService:BaseAddress"];
        if (productServiceUrl != null) c.BaseAddress = new Uri(productServiceUrl);
    })
    .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

// Add shared infrastructure services (includes ICurrentUserService)
builder.Services.AddSharedInfrastructure();
builder.Services.AddSwaggerDocs("Rental Service");

// EF Core registration specific to the service
builder.Services.AddDbContext<RentalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSaanjhiHealthChecks(builder.Configuration)
    .AddSaanjhiServiceHealthCheck("Product Service",
        builder.Configuration["HttpClient:ProductService:BaseAddress"] ?? string.Empty);

var app = builder.Build();
app.ApplyMigrations<RentalDbContext>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds
            })
        });
        await context.Response.WriteAsync(result);
    }
});
// Use CORS policy
app.UseCors("AllowAll");
app.UseMiddleware<CustomExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();