using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.HttpClients;
using ProductService.Infrastructure.HttpHandlers;
using Shared.ErrorHandling;
using Shared.Extensions.Telemetry;
using ProductService.Infrastructure.Persistence;
using Shared.Extensions;
using Shared.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

var appAssembly = Assembly.Load("ProductService.Application");

builder.UseSharedSentry();
builder.Services.AddSharedTelemetry(builder.Configuration, "ProductService");
// Common shared service registration
builder.Services.AddApplicationServices(appAssembly, builder.Configuration);
builder.Services.AddTransient<AuthenticatedHttpClientHandler>();
builder.Services.AddTransient<ITokenProvider, TokenProvider>();
builder.Services.AddHttpClient<CategoryApiClient>(c =>
    {
        var categoryServiceUrl = builder.Configuration["HttpClient:CategoryService:BaseAddress"];
        if (categoryServiceUrl != null) c.BaseAddress = new Uri(categoryServiceUrl);
    })
    .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();
builder.Services.AddSwaggerDocs("Product Service");

// EF Core registration specific to the service
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSaanjhiHealthChecks(builder.Configuration).AddSaanjhiServiceHealthCheck("Category Service", 
    builder.Configuration["HttpClient:CategoryService:BaseAddress"] ?? string.Empty);
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
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();