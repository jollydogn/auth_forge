using System;
using System.Collections.Generic;

namespace AuthForge.Entities;

/// <summary>
/// Persistent permission group stored in the consumer's database.
/// Seeded automatically on application startup from PermissionDefinitionProviders.
/// </summary>
public class AuthForgePermissionGroup
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Unique group name (e.g., "OrderManagement").
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Display name for UI.
    /// </summary>
    public string? DisplayName { get; set; }

    public virtual ICollection<AuthForgePermission> Permissions { get; set; } = new List<AuthForgePermission>();
}
