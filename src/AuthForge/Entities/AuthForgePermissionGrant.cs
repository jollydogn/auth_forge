using System;

namespace AuthForge.Entities;

/// <summary>
/// Represents a permission grant to either a Role or a specific User.
/// This is the "who has what permission" mapping table.
/// </summary>
public class AuthForgePermissionGrant
{
    public Guid Id { get; set; }

    /// <summary>
    /// FK to the permission being granted.
    /// </summary>
    public Guid PermissionId { get; set; }
    public virtual AuthForgePermission Permission { get; set; } = default!;

    /// <summary>
    /// The type of the provider: "Role" or "User".
    /// </summary>
    public string ProviderName { get; set; } = default!;

    /// <summary>
    /// The identifier of the provider.
    /// If ProviderName = "Role", this is the role name (e.g., "Admin").
    /// If ProviderName = "User", this is the user ID (Guid as string).
    /// </summary>
    public string ProviderKey { get; set; } = default!;
}
