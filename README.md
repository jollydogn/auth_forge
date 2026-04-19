# AuthForge 🛡️

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-AuthForge-blue?style=flat-square&logo=nuget)](https://www.nuget.org/packages/AuthForge)
[![Build](https://github.com/jollydogn/auth_forge/actions/workflows/pr-validation.yml/badge.svg)](https://github.com/jollydogn/auth_forge/actions)

A professional, plug-and-play **Keycloak Authentication and Authorization SDK** for ASP.NET Core **(.NET 10.0 Only)**. 
AuthForge strictly follows SOLID principles and allows you to seamlessly integrate Keycloak identity management, token fetching, group/role mappings, and dynamic native `.NET Claim` transformations — all via a clean, Dependency-Injected API. 

---

## ✨ Features

- 🎯 **Native Authorization** — Transforms complex Keycloak `realm_access` JSON to native `.NET Claims` automatically.
- 🛡️ **Custom Authorization** — Use `[AuthForgeAuthorize(Roles = "...")]` to secure your REST endpoints effortlessly.
- 🔑 **Token Management** — Fast HTTP clients for `IAuthManager` (Login, Refresh Token, Logout).
- 👥 **User API (Admin)** — Manage Keycloak users directly via `IUsersManager` (Auto Resets, Execute Actions Email).
- 🎭 **Roles & Groups** — Organize users and manage realm policies with `IRolesManager` and `IGroupsManager`.
- 👤 **ICurrentUser Wrapper** — Extracts user data safely from tokens (Id, Name, Roles) without traversing HttpContext.
- ⚡ **Auto Caching** — Intelligent MemoryCache integration for Keycloak Admin API tokens.
- 🧩 **One-Line Startup** — Boot in one line: `builder.Services.AddAuthForge(Configuration)`.

---

## 📦 Installation

```bash
dotnet add package AuthForge
```

Or via Package Manager:

```powershell
Install-Package AuthForge
```

---

## 🚀 Quick Start

### 1. Configure `appsettings.json`

Add the Keycloak URLs and target Audience:

```json
{
  "AuthForge": {
    "Authority": "http://localhost:8080/realms/master",
    "Audience": "account",
    "RequireHttpsMetadata": false,
    "MapRealmRolesToClaims": true,
    "MapClientRolesToClaims": false
  },
  "AuthForgeAdmin": {
    "BaseUrl": "http://localhost:8080",
    "Realm": "master",
    "AdminRealm": "master",
    "ClientId": "admin-cli",
    "ClientSecret": "",
    "LoginClientId": "frontend-client",
    "TokenCacheDurationSeconds": 50
  }
}
```

### 2. Register Services in `Program.cs`

AuthForge registers memory cache, HTTP clients, JWT standard bearers, and manager abstractions out of the box.

```csharp
using AuthForge.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { ... });

// 🪄 Register AuthForge layers and JWT Auth
builder.Services.AddAuthForge(builder.Configuration);

var app = builder.Build();

// 🛡️ Activates Authentication & Authorization middleware
app.UseAuthForge();

app.MapControllers();
app.Run();
```

---

## 🛡️ Usage and Controllers

AuthForge translates Keycloak's Role Arrays into fully supported `.NET Claims`. You can restrict your routes simply by using strings:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // Restricts execution to any Keycloak User possessing 'admin' OR 'product-manager' role
    [AuthForgeAuthorize(Roles = "admin, product-manager")]
    [HttpPost("create")]
    public IActionResult CreateProduct()
    {
        return Ok(new { message = "Product created successfully!" });
    }
}
```

---

## 👤 Current User Service (No HttpContext logic!)

Stop manually ripping the `Auth Header` or dealing with Keycloak formats. Inject `ICurrentUser` anywhere globally.

```csharp
public class AnalyticsService
{
    private readonly ICurrentUser _currentUser;
    
    public AnalyticsService(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }
    
    public void TrackActivity()
    {
        if (_currentUser.IsAuthenticated) 
        {
            var email = _currentUser.Email;
            var isPowerUser = _currentUser.IsInRole("power-user");
        }
    }
}
```

---

## 🗄️ Local Database Synchronization (Entity Framework Core)

AuthForge uniquely bridges the gap between Keycloak's isolated databases and your native application. By utilizing **Local User Synchronization**, you can spawn `AuthForgeUser`, `AuthForgeRole`, and `AuthForgeGroup` physical tables directly inside your SQL Server or PostgreSQL database. This allows you to execute native `LINQ Queries`, perform `.Include()` join operations on your custom tables (like Orders and Carts), and query without ever bottlenecking Keycloak!

Add the sets to your DbContext and inject the schema builder:

```csharp
using AuthForge.EntityFrameworkCore;
using AuthForge.Entities;

public class AppDbContext : DbContext
{
    public DbSet<AuthForgeUser> AuthForgeUsers { get; set; }
    public DbSet<AuthForgeRole> AuthForgeRoles { get; set; }
    public DbSet<AuthForgeGroup> AuthForgeGroups { get; set; }
    public DbSet<AuthForgeUserRole> AuthForgeUserRoles { get; set; }
    public DbSet<AuthForgeUserGroup> AuthForgeUserGroups { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Magically configures the mappings, Primary Keys, Maximum Lengths, and cascade limits!
        builder.ConfigureAuthForge(tablePrefix: "AuthForge_", schema: null);
    }
}
```

Simply run `dotnet ef migrations add AddAuthForgeEntities` and keep them in-sync with your webhooks or Managers!

---

## 🛡️ ABP-Style Permission System

AuthForge comes with a built-in, code-first permission definition system inspired by the **ABP Framework**. This allows you to define your permissions and groups purely in C# code. On startup, these permissions are automatically seeded into your Local Synchronization Database.

### 1. Define Your Permissions

Create a class that inherits from `AuthForgePermissionDefinitionProvider` and override the `Define` method:

```csharp
using AuthForge.Permissions;

public class OrderPermissionDefinitionProvider : AuthForgePermissionDefinitionProvider
{
    public override void Define(AuthForgePermissionDefinitionContext context)
    {
        // 1. Create a logical group
        var orderGroup = context.AddGroup("OrderManagement", "Order Management");
        
        // 2. Add top-level permission
        var rootPermission = orderGroup.AddPermission("Orders", "Orders Access");
        
        // 3. Add fine-grained child permissions
        rootPermission.AddChild("Orders.Create", "Create Order");
        rootPermission.AddChild("Orders.Edit", "Edit Order");
        rootPermission.AddChild("Orders.Delete", "Delete Order");
    }
}
```

### 2. Register the Permission System

In your `Program.cs`, simply call `AddAuthForgePermissions` and pass your local `DbContext`. This will automatically scan your assembly for providers and register the Auto-Seeder background service.

```csharp
// Scan the current assembly and seed into AppDbContext
builder.Services.AddAuthForgePermissions<AppDbContext>(typeof(Program).Assembly);
```

### 3. Protect Your APIs

Use the `Permissions` alias inside the custom `[AuthForgeAuthorize]` attribute to explicitly enforce these permissions in your endpoints:

```csharp
[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase 
{
    [HttpPost("create")]
    // Semantic translation: Checks if the user's Token or Local DB mapping contains "Orders.Create"
    [AuthForgeAuthorize(Permissions = "Orders.Create")]
    public IActionResult Create() => Ok();
}
```

### 4. Assign Permissions (Grants)

You can assign these granular permissions directly to Users or master Roles (Composite Role pattern) via the `IPermissionsManager`.

```csharp
// Assign "Orders.Create" permission to everyone who holds the "Manager" role
await _permissionsManager.AssignPermissionToRoleAsync(
    roleName: "Manager", 
    permissionName: "Orders.Create"
);
```

---

## 🔧 Core Domain Managers

AuthForge provides robust SDK Managers to connect directly to the underlying Keycloak REST/Admin REST API. All of them are `virtual` methods—easily overriden if you require special company mappings!

### Auth Manager
Retrieve login tokens via Password Granter, or issue sign-outs:
```csharp
var token = await _authManager.LoginByUsernameAsync(new LoginByUsernameRequest { 
    Username = "admin", 
    Password = "123" 
});

await _authManager.LogoutAsync(new LogoutRequest { RefreshToken = token.RefreshToken });
```

### Users Manager
Act as an administrator: Creating, Updating, getting Users directly:
```csharp
var newUserId = await _usersManager.CreateUserAsync(new CreateUserRequest {
    Username = "jdoe",
    Email = "jdoe@company.com",
    InitialPassword = "TempPassword1!",
    IsTemporaryPassword = true
});

// Trigger Keycloak to email them to verify address!
await _usersManager.ExecuteActionsEmailAsync(newUserId, new List<string> { "VERIFY_EMAIL" });
```

### Roles & Groups Manager
Read users' active roles or bind them cleanly via code:
```csharp
await _groupsManager.AddUserToGroupAsync(userId, engineeringGroupId);
await _rolesManager.AssignRoleToUserAsync(userId, new RoleRequest { Id = roleId, Name = "dev" });
```

---

## 📁 Project Structure

```
AuthForge/
├── src/
│   ├── AuthForge/                     # Core NuGet library
│   │   ├── Authentication/           # KeycloakClaimsTransformation
│   │   ├── Authorization/            # AuthForgeAuthorizeAttribute
│   │   ├── Clients/                  # Admin REST HttpClient & MemoryCache
│   │   ├── Configuration/            # Core Options Bindings
│   │   ├── Extensions/               # DI Extensions (AddAuthForge, UseAuthForge)
│   │   ├── Managers/                 # Auth, Users, Groups, Roles (Interfaces & Logic)
│   │   └── Models/                   # Token, Requests, Responses (DTOs)
│   │
│   ├── AuthForge.Sample/             # Demo API Example
│   │   ├── Controllers/              # Swagger endpoints and role tests
│   │   └── Program.cs                
│   │
│   └── AuthForge.Tests/              # xUnit Test project
│
├── .github/workflows/                # CI CD GitHub Actions (PR Validation, NuGet Publish)
└── README.md
```

---

## 🤝 Contributing

Contributions are welcome! Allow AuthForge to cover more Keycloak flows by submitting a PR.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🔗 Links

- [GitHub Repository](https://github.com/jollydogn/auth_forge)
- [Report an Issue](https://github.com/jollydogn/auth_forge/issues)

---

Made with 🛡️ by [AuthForge](https://github.com/jollydogn)
