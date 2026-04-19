using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthForge.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthForge.Authorization;

/// <summary>
/// Custom authorization requirement for permission-based checks.
/// </summary>
public class AuthForgePermissionRequirement : IAuthorizationRequirement
{
    public string PermissionName { get; }

    public AuthForgePermissionRequirement(string permissionName)
    {
        PermissionName = permissionName;
    }
}

/// <summary>
/// Authorization handler that checks whether the current user (or their roles)
/// have been granted a specific permission via the AuthForge_PermissionGrants table.
/// </summary>
public class AuthForgePermissionAuthorizationHandler<TDbContext> : AuthorizationHandler<AuthForgePermissionRequirement>
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    public AuthForgePermissionAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthForgePermissionRequirement requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
            return;

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? context.User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return;

        var userRoles = context.User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Check grants: either directly to this user OR to any of their roles
        var hasGrant = await dbContext.Set<AuthForgePermissionGrant>()
            .Include(g => g.Permission)
            .AnyAsync(g =>
                g.Permission.Name == requirement.PermissionName &&
                (
                    (g.ProviderName == "User" && g.ProviderKey == userId) ||
                    (g.ProviderName == "Role" && userRoles.Contains(g.ProviderKey))
                ));

        if (hasGrant)
        {
            context.Succeed(requirement);
        }
    }
}
