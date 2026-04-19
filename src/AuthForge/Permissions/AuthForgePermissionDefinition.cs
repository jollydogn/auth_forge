using System.Collections.Generic;

namespace AuthForge.Permissions;

/// <summary>
/// Represents a single permission definition (e.g., "Orders.Create").
/// Permissions can have child permissions for hierarchical organization.
/// </summary>
public class AuthForgePermissionDefinition
{
    /// <summary>
    /// Unique permission name (e.g., "Orders.Create").
    /// This is the key used in [AuthForgeAuthorize(Permissions = "...")].
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Display name shown in management UIs.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Parent group this permission belongs to.
    /// </summary>
    public AuthForgePermissionGroupDefinition Group { get; }

    /// <summary>
    /// Parent permission (null if this is a top-level permission within the group).
    /// </summary>
    public AuthForgePermissionDefinition? Parent { get; }

    /// <summary>
    /// Child permissions for hierarchical structure.
    /// </summary>
    public IReadOnlyList<AuthForgePermissionDefinition> Children => _children;
    private readonly List<AuthForgePermissionDefinition> _children = new();

    public AuthForgePermissionDefinition(
        string name,
        string? displayName,
        AuthForgePermissionGroupDefinition group,
        AuthForgePermissionDefinition? parent = null)
    {
        Name = name;
        DisplayName = displayName ?? name;
        Group = group;
        Parent = parent;
    }

    /// <summary>
    /// Adds a child permission under this permission.
    /// Example: "Orders" -> AddChild("Orders.Create") -> AddChild("Orders.Delete")
    /// </summary>
    public AuthForgePermissionDefinition AddChild(string name, string? displayName = null)
    {
        var child = new AuthForgePermissionDefinition(name, displayName, Group, this);
        _children.Add(child);
        return child;
    }
}
