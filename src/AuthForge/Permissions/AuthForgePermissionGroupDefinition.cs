using System.Collections.Generic;

namespace AuthForge.Permissions;

/// <summary>
/// Represents a logical permission group (e.g., "OrderManagement", "ArticleManagement").
/// All permissions must belong to a group. Groups are used for UI organization and bulk management.
/// </summary>
public class AuthForgePermissionGroupDefinition
{
    /// <summary>
    /// Unique name of the permission group (e.g., "OrderManagement").
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Display name shown in management UIs.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Permissions belonging to this group.
    /// </summary>
    public IReadOnlyList<AuthForgePermissionDefinition> Permissions => _permissions;
    private readonly List<AuthForgePermissionDefinition> _permissions = new();

    public AuthForgePermissionGroupDefinition(string name, string? displayName = null)
    {
        Name = name;
        DisplayName = displayName ?? name;
    }

    /// <summary>
    /// Adds a new permission to this group.
    /// Returns the created permission so you can chain AddChild calls.
    /// </summary>
    public AuthForgePermissionDefinition AddPermission(string name, string? displayName = null)
    {
        var permission = new AuthForgePermissionDefinition(name, displayName, this);
        _permissions.Add(permission);
        return permission;
    }
}
