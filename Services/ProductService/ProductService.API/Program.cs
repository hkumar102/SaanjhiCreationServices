using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.HttpClients;
using ProductService.Infrastructure.HttpHandlers;
using ProductService.Application.Services;
using Shared.Extensions.Telemetry;
using ProductService.Infrastructure.Persistence;
using Shared.Extensions;
using Shared.HealthChecks;
using Shared.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Debug);

var appAssembly = Assembly.Load("ProductService.Application");

builder.UseSharedSentry();
builder.Services.AddSharedTelemetry(builder.Configuration, "ProductService");

// Add shared infrastructure services (includes ICurrentUserService)
builder.Services.AddSharedInfrastructure();

// Common shared service registration
builder.Services.AddApplicationServices(appAssembly, builder.Configuration);
builder.Services.AddTransient<AuthenticatedHttpClientHandler>();
builder.Services.AddTransient<ITokenProvider, TokenProvider>();

// MediaService HTTP client
builder.Services.AddHttpClient<IMediaServiceClient, MediaServiceClient>(client =>
{
    var mediaServiceBaseUrl = builder.Configuration["Services:MediaService:BaseUrl"] ?? "http://localhost:5003";
    client.BaseAddress = new Uri(mediaServiceBaseUrl);
    client.Timeout = TimeSpan.FromMinutes(5); // Allow for large file uploads
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

// CategoryApiClient removed - categories are now handled locally
builder.Services.AddSwaggerDocs("Product Service");

// EF Core registration specific to the service
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSaanjhiHealthChecks(builder.Configuration).AddSaanjhiServiceHealthCheck("Media Service", 
    builder.Configuration["Services:MediaService:BaseUrl"] ?? string.Empty);
builder.Services.AddAutoMapper(appAssembly);

var app = builder.Build();
app.ApplyMigrations<ProductDbContext>();
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
            checks = report.Entries.Select(e => new {
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
// app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<Shared.ErrorHandling.CustomExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();