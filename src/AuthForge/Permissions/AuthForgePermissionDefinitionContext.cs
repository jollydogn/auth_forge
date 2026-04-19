using System.Collections.Generic;

namespace AuthForge.Permissions;

/// <summary>
/// Context passed to PermissionDefinitionProviders during the Define phase.
/// Collects all groups and their permissions from all providers.
/// </summary>
public class AuthForgePermissionDefinitionContext
{
    public IReadOnlyList<AuthForgePermissionGroupDefinition> Groups => _groups;
    private readonly List<AuthForgePermissionGroupDefinition> _groups = new();

    /// <summary>
    /// Creates a new permission group.
    /// </summary>
    /// <param name="name">Unique group name (e.g., "OrderManagement")</param>
    /// <param name="displayName">UI-friendly display name</param>
    public AuthForgePermissionGroupDefinition AddGroup(string name, string? displayName = null)
    {
        var group = new AuthForgePermissionGroupDefinition(name, displayName);
        _groups.Add(group);
        return group;
    }

    /// <summary>
    /// Gets a previously registered group by name, or null if not found.
    /// Useful when multiple providers need to add permissions to the same group.
    /// </summary>
    public AuthForgePermissionGroupDefinition? GetGroupOrNull(string name)
    {
        return _groups.Find(g => g.Name == name);
    }
}
