using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Extensions.Telemetry;
using UserService.Infrastructure.Persistence;
using Shared.HealthChecks;

var builder = WebApplication.CreateBuilder(args);


builder.UseSharedSentry();
builder.Services.AddSharedTelemetry(builder.Configuration, "UserService");
// Common shared service registration
var appAssembly = "RentalService.Application";
builder.Services.RegisterAssemblyServices(appAssembly);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSwaggerDocs("User Service");

// EF Core registration specific to the service
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSaanjhiHealthChecks(builder.Configuration);

var app = builder.Build();
app.ApplyMigrations<UserDbContext>();
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

