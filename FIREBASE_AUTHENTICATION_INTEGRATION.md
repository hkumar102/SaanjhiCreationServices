# Firebase Authentication Integration with Current User Service

## Overview
Enhanced the existing Firebase authentication to work seamlessly with the audit trail system, capturing detailed user information from Firebase tokens.

## Enhanced FirebaseAuthenticationHandler

### Claims Added
The `FirebaseAuthenticationHandler` now extracts and includes these claims:

```csharp
// Standard JWT claims
new("sub", decoded.Uid)                    // Standard JWT subject
new("uid", decoded.Uid)                    // Firebase UID
new(ClaimTypes.NameIdentifier, decoded.Uid) // .NET standard claim

// User profile claims
new(ClaimTypes.Email, email)               // User email
new(ClaimTypes.Name, name)                 // User display name
new("email", email)                        // JWT email claim
new("name", name)                          // JWT name claim

// Firebase-specific claims
new("sign_in_provider", provider)          // google.com, password, etc.
new(ClaimTypes.Role, role)                 // User roles
new("custom_*", customValue)               // Custom claims

// Security claims
new("iss", decoded.Issuer)                 // Token issuer
new("aud", decoded.Audience)               // Token audience
```

## Enhanced CurrentUserService

### Available Properties
```csharp
public interface ICurrentUserService
{
    string? UserId { get; }              // Firebase UID
    string? UserName { get; }            // Display name or email
    string? Email { get; }               // User email
    string? SignInProvider { get; }      // Authentication method
    bool IsAuthenticated { get; }        // Authentication status
    IEnumerable<string> Roles { get; }   // User roles
    
    bool HasRole(string role);           // Role checking
    string? GetCustomClaim(string type); // Custom claims access
}
```

## Usage Examples

### In Controllers
```csharp
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductCommand command)
    {
        // Automatically captured in audit fields via DbContext
        var result = await _mediator.Send(command);
        
        // Manual access if needed
        _logger.LogInformation("Product created by {UserId} ({Email})", 
            _currentUser.UserId, _currentUser.Email);
            
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        if (!_currentUser.HasRole("admin"))
            return Forbid();
            
        // Soft delete will capture user info
        await _mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}
```

### In Command Handlers
```csharp
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly ProductDbContext _context;
    private readonly ICurrentUserService _currentUser;
    
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product 
        { 
            Name = request.Name,
            // Audit fields automatically set on SaveChanges
        };
        
        _context.Products.Add(product);
        
        // User info automatically captured in audit fields:
        // CreatedBy = Firebase UID
        // CreatedAt = Current timestamp
        await _context.SaveChangesAsync(cancellationToken);
        
        return product.Id;
    }
}
```

### Custom Authorization
```csharp
public class RequireRoleAttribute : AuthorizeAttribute
{
    public RequireRoleAttribute(string role) : base()
    {
        Roles = role;
    }
}

// Usage
[HttpPost("admin-only")]
[RequireRole("admin")]
public async Task<IActionResult> AdminOnlyAction() { }
```

### Access Custom Claims
```csharp
public class UserService
{
    private readonly ICurrentUserService _currentUser;
    
    public async Task<UserProfile> GetUserProfile()
    {
        return new UserProfile
        {
            Id = _currentUser.UserId,
            Email = _currentUser.Email,
            Name = _currentUser.UserName,
            Provider = _currentUser.SignInProvider,
            Subscription = _currentUser.GetCustomClaim("subscription_type"),
            TenantId = _currentUser.GetCustomClaim("tenant_id"),
            Roles = _currentUser.Roles.ToList()
        };
    }
}
```

## Firebase Custom Claims Setup

### Setting Custom Claims (Firebase Admin SDK)
```javascript
// In your Firebase Functions or admin service
await admin.auth().setCustomUserClaims(uid, {
  role: 'admin',
  subscription_type: 'premium',
  tenant_id: 'company_123',
  permissions: ['read_products', 'write_products']
});
```

### Setting Roles in Firebase
```javascript
// Single role
await admin.auth().setCustomUserClaims(uid, { role: 'admin' });

// Multiple roles
await admin.auth().setCustomUserClaims(uid, { 
  roles: ['user', 'moderator'] 
});
```

## Audit Trail Examples

### Database Records
With the enhanced system, audit trails now show:

```sql
-- Products table
Id: 123e4567-e89b-12d3-a456-426614174000
Name: "Evening Dress"
CreatedAt: 2025-07-11 14:30:00
CreatedBy: "firebase_user_abc123"      -- Real Firebase UID
ModifiedAt: 2025-07-11 15:45:00
ModifiedBy: "firebase_user_xyz789"     -- User who modified
IsDeleted: false
```

### Soft Delete Tracking
```csharp
// When a user soft deletes a product
product.SoftDelete(_currentUser.UserId);
await _context.SaveChangesAsync();

// Results in:
// IsDeleted: true
// DeletedAt: 2025-07-11 16:00:00
// DeletedBy: "firebase_user_abc123"
```

## Security Benefits

1. **Complete Audit Trail** - Every change tracked to specific Firebase users
2. **Role-Based Access** - Leverage Firebase custom claims for authorization
3. **Multi-Tenant Support** - Custom claims can include tenant information
4. **Provider Tracking** - Know if user signed in via Google, email, etc.
5. **Custom Permissions** - Flexible authorization with custom claims

## Testing

### Mock for Unit Tests
```csharp
var mockCurrentUser = new Mock<ICurrentUserService>();
mockCurrentUser.Setup(x => x.UserId).Returns("test_user_123");
mockCurrentUser.Setup(x => x.Email).Returns("test@example.com");
mockCurrentUser.Setup(x => x.HasRole("admin")).Returns(true);

// Use in tests
var handler = new CreateProductCommandHandler(context, mockCurrentUser.Object);
```

### Integration Test Setup
```csharp
// Create test Firebase token with custom claims
var customClaims = new Dictionary<string, object>
{
    ["role"] = "admin",
    ["tenant_id"] = "test_tenant"
};

var testToken = await FirebaseAuth.DefaultInstance
    .CreateCustomTokenAsync("test_user", customClaims);
```

This integration provides a robust, secure foundation for tracking user actions throughout your clothing rental platform! 🔐🚀
