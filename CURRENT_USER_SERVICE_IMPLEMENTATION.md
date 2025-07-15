# Current User Service Implementation

## The Problem
In the ProductDbContext, we were hardcoding "system" as the user for audit fields:

```csharp
this.UpdateAuditFields("system"); // Not ideal!
```

## Solutions Implemented

### Option 1: ICurrentUserService (Recommended) ✅

**Benefits:**
- Clean separation of concerns
- Easy to test and mock
- Works with any authentication system
- Consistent across all services

**Implementation:**
```csharp
public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ??
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub") ??
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
    // ... other properties
}
```

**DbContext Usage:**
```csharp
public class ProductDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUser = _currentUserService?.UserId ?? "system";
        this.UpdateAuditFields(currentUser);
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

**Registration:**
```csharp
// In Program.cs
builder.Services.AddSharedInfrastructure(); // Includes ICurrentUserService
```

### Option 2: DbContext with User Parameter

```csharp
public async Task<int> SaveChangesAsync(string? currentUser = null, CancellationToken cancellationToken = default)
{
    this.UpdateAuditFields(currentUser ?? "system");
    return await base.SaveChangesAsync(cancellationToken);
}
```

**Usage in Services:**
```csharp
await _context.SaveChangesAsync(currentUser.UserId);
```

### Option 3: Custom DbContext with User Context

```csharp
public class ProductDbContext : DbContext
{
    public string? CurrentUser { get; set; }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.UpdateAuditFields(CurrentUser ?? "system");
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

**Usage:**
```csharp
_context.CurrentUser = currentUserService.UserId;
await _context.SaveChangesAsync();
```

## Authentication Integration

### For JWT/Firebase Auth
The `CurrentUserService` automatically extracts user info from JWT claims:
- `sub` or `uid` - User identifier
- `email` - User email
- `name` - User name

### For API Keys/Service-to-Service
```csharp
public class ApiKeyCurrentUserService : ICurrentUserService
{
    public string? UserId => "api-service";
    public string? UserName => "API Service";
    public bool IsAuthenticated => true;
}
```

### For Background Jobs
```csharp
public class SystemCurrentUserService : ICurrentUserService
{
    public string? UserId => "system";
    public string? UserName => "System Process";
    public bool IsAuthenticated => true;
}
```

## Testing
Easy to mock for unit tests:
```csharp
var mockCurrentUser = new Mock<ICurrentUserService>();
mockCurrentUser.Setup(x => x.UserId).Returns("test-user-123");

var context = new ProductDbContext(options, mockCurrentUser.Object);
```

## Best Practices

1. **Always provide fallback** to "system" for migrations/seeding
2. **Use dependency injection** for clean architecture
3. **Handle null cases** gracefully
4. **Consider different auth scenarios** (JWT, API keys, background jobs)
5. **Make it testable** with interfaces and mocking

## Fallback Strategy
```csharp
var currentUser = _currentUserService?.UserId ?? 
                  _httpContext?.User?.FindFirstValue("sub") ?? 
                  "system";
```

This ensures audit fields are always populated, even when:
- Running migrations
- Seeding data
- Background processes
- Integration tests
- System operations

The implemented solution provides a clean, testable, and maintainable way to track who performs operations on your entities! 🎯
