using System.Reflection;
using System.Text.Json;
using MediaService.Application.Interfaces;
using MediaService.Application.Services;
using MediaService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Shared.ErrorHandling;
using Shared.Extensions;
using Shared.Extensions.Telemetry;
using Shared.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

var appAssembly = Assembly.Load("MediaService.Application");

builder.UseSharedSentry();
builder.Services.AddSharedTelemetry(builder.Configuration, "MediaService");
// Common shared service registration
builder.Services.AddApplicationServices(appAssembly, builder.Configuration);
builder.Services.AddHttpClient<IMediaUploader, ImgurMediaUploader>();
builder.Services.AddSwaggerDocs("Media Service");

// EF Core registration specific to the service
builder.Services.AddDbContext<MediaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSaanjhiHealthChecks(builder.Configuration);
builder.Services.AddAutoMapper(appAssembly);

var app = builder.Build();
app.ApplyMigrations<MediaDbContext>();
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