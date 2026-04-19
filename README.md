# AuthForge

A robust, plug-and-play **Keycloak Authentication and Authorization SDK** for ASP.NET Core (.NET 8 & 9).
AuthForge strictly follows SOLID principles and allows you to seamlessly integrate Keycloak identity management, Token fetching, Group/Role mappings, and `[AuthForgeAuthorize(Roles="...")]` policies directly into your applications without manually tracking scopes or building HTTP wrappers.

## 🚀 Installation

Install the package via NuGet:
```bash
dotnet add package AuthForge
```

## 🛠️ Quick Start

**1. Configure your `appsettings.json`:**
```json
{
  "AuthForge": {
    "Authority": "http://localhost:8080/realms/master/",
    "Audience": "account",
    "RequireHttpsMetadata": false,
    "MapRealmRolesToClaims": true,
    "RoleClaimType": "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
  },
  "AuthForgeAdmin": {
    "BaseUrl": "http://localhost:8080",
    "Realm": "master",
    "AdminRealm": "master",
    "ClientId": "admin-cli",
    "ClientSecret": "your-client-secret-if-any",
    "LoginClientId": "frontend-client",
    "TokenCacheDurationSeconds": 50
  }
}
```

**2. Register Services in `Program.cs`:**
```csharp
using AuthForge.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Adds DI mappings, Caching, and JWT Bearer Options automatically.
builder.Services.AddAuthForge(builder.Configuration);

var app = builder.Build();

// Uses Authentication and Authorization standard middlewares
app.UseAuthForge();
```

## 🛡️ Usage and Best Practices

### Role-Based Access Control
AuthForge dynamically transforms Keycloak's `realm_access` and `resource_access` arrays into native `.NET Claims`. You can protect your Controllers using:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // Restricts to any Keycloak user with 'admin' or 'product-manager' role
    [AuthForgeAuthorize(Roles = "admin, product-manager")]
    [HttpPost]
    public IActionResult Create() => Ok();
}
```

### Dependency Injection (ICurrentUser)
Easily inject `ICurrentUser` to access data anywhere (No need to parse HttpContext yourself!)

```csharp
public class AnalyticsService
{
    private readonly ICurrentUser _currentUser;
    
    public AnalyticsService(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }
    
    public void Track()
    {
        var email = _currentUser.Email;
        var isAdmin = _currentUser.IsInRole("admin");
    }
}
```

### Fully Featured Managers
AuthForge provides multiple layers to directly interact with Keycloak. Use these inside your Business Logic (not your Controllers!):
- `IAuthManager`: Generate Tokens (LoginByUsername, Mobile, Email), Refresh, and Logout operations.
- `IUsersManager`: Admin-level Create User, Update, Reset Passwords, or send Keycloak Email Verification.
- `IRolesManager`: Assign Realm roles manually, fetch all roles.
- `IGroupsManager`: Manage organizational sub-groups natively.

### Extensibility (Virtual overriding)
Every `Manager` implementation is defined using `virtual` modifiers (`public virtual Task...`). You can easily extend them in your own application by subclassing them if you need complex business mapping.

## CI/CD and Contribution
This repository utilizes standard Conventional Commits. Commits are automatically evaluated and pushed directly to NuGet on merging to the `main` branch.
