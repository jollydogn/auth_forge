using System.Collections.Generic;
using System.Linq;

namespace AuthForge.Permissions;

/// <summary>
/// Collects all registered PermissionDefinitionProviders, runs their Define methods,
/// and builds the complete permission tree. This is the runtime registry.
/// </summary>
public interface IAuthForgePermissionDefinitionManager
{
    IReadOnlyList<AuthForgePermissionGroupDefinition> GetGroups();
    IReadOnlyList<AuthForgePermissionDefinition> GetAllPermissions();
    AuthForgePermissionDefinition? GetOrNull(string name);
}

public class AuthForgePermissionDefinitionManager : IAuthForgePermissionDefinitionManager
{
    private readonly AuthForgePermissionDefinitionContext _context;
    private List<AuthForgePermissionDefinition>? _flatPermissionsCache;

    public AuthForgePermissionDefinitionManager(
        IEnumerable<AuthForgePermissionDefinitionProvider> providers)
    {
        _context = new AuthForgePermissionDefinitionContext();

        foreach (var provider in providers)
        {
            provider.Define(_context);
        }
    }

    public IReadOnlyList<AuthForgePermissionGroupDefinition> GetGroups()
    {
        return _context.Groups;
    }

    public IReadOnlyList<AuthForgePermissionDefinition> GetAllPermissions()
    {
        return _flatPermissionsCache ??= BuildFlatList();
    }

    public AuthForgePermissionDefinition? GetOrNull(string name)
    {
        return GetAllPermissions().FirstOrDefault(p => p.Name == name);
    }

    private List<AuthForgePermissionDefinition> BuildFlatList()
    {
        var result = new List<AuthForgePermissionDefinition>();

        foreach (var group in _context.Groups)
        {
            foreach (var permission in group.Permissions)
            {
                CollectRecursive(permission, result);
            }
        }

        return result;
    }

    private static void CollectRecursive(
        AuthForgePermissionDefinition permission,
        List<AuthForgePermissionDefinition> result)
    {
        result.Add(permission);
        foreach (var child in permission.Children)
        {
            CollectRecursive(child, result);
        }
    }
}
