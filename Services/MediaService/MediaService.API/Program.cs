using System.Reflection;
using System.Text.Json;
using MediaService.Application.Services;
using MediaService.Contracts.Interfaces;
using MediaService.Infrastructure.Persistence;
using MediaService.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Extensions.Telemetry;
using Shared.HealthChecks;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var appAssembly = Assembly.Load("MediaService.Application");

builder.UseSharedSentry();
builder.Services.AddSharedTelemetry(builder.Configuration, "MediaService");

// Common shared service registration
builder.Services.AddApplicationServices(appAssembly, builder.Configuration);

// MediaService specific services
builder.Services.AddScoped<IMediaUploader, ImageKitMediaUploader>();
builder.Services.AddScoped<IImageProcessingService, ImageProcessingService>();

// Always register ImageKitUrlService since handlers might use it
builder.Services.AddScoped<ImageKitUrlService>();

// File storage service - environment based
var useLocalStorage = builder.Configuration.GetValue<bool>("FileStorage:UseLocal", false);
if (useLocalStorage)
{
    Console.WriteLine("Using LocalFileStorageService for file storage");
    builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
}
else
{
    Console.WriteLine("Using ImageKitFileStorageService for file storage");
    builder.Services.AddScoped<IFileStorageService, ImageKitFileStorageService>();
}

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

// Configure static files for local storage
var isLocalStorageEnabled = builder.Configuration.GetValue<bool>("FileStorage:UseLocal", false);
if (isLocalStorageEnabled || app.Environment.IsDevelopment())
{
    var configuredPath = builder.Configuration["FileStorage:LocalPath"] ?? "./wwwroot/uploads";
    var storagePath = Path.IsPathRooted(configuredPath) 
        ? configuredPath 
        : Path.Combine(Directory.GetCurrentDirectory(), configuredPath.TrimStart('.', '/'));
    
    // Ensure uploads directory exists
    Directory.CreateDirectory(storagePath);
    
    Console.WriteLine($"Static files served from: {storagePath}");
    
    // Serve static files from uploads directory
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(storagePath),
        RequestPath = "/uploads"
    });
}

app.Run();