using System.Reflection;
using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Persistence;
using Shared.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var appAssembly = Assembly.Load("UserService.Application");

// Common shared service registration
builder.Services.AddApplicationServices(appAssembly);
builder.Services.AddSwaggerDocs("User Service");

// EF Core registration specific to the service
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();

