using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Shared.ErrorHandling;
using Shared.Extensions.Telemetry;
using CustomerService.Infrastructure.Persistence;
using Shared.Extensions;
using Shared.Infrastructure.Extensions;
using Shared.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

var appAssembly = Assembly.Load("CustomerService.Application");

builder.UseSharedSentry();
builder.Services.AddSharedTelemetry(builder.Configuration, "CustomerService");
// Common shared service registration
builder.Services.AddApplicationServices(appAssembly);
builder.Services.AddSwaggerDocs("Customer Service");

// EF Core registration specific to the service
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSaanjhiHealthChecks(builder.Configuration);
builder.Services.AddAutoMapper(appAssembly);

var app = builder.Build();
app.ApplyMigrations<CustomerDbContext>();
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

app.UseHttpsRedirection();
app.MapControllers();
app.Run();