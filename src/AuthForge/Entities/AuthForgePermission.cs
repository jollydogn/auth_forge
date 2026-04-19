using System;
using System.Collections.Generic;

namespace AuthForge.Entities;

/// <summary>
/// Persistent permission definition stored in the consumer's database.
/// Seeded automatically on application startup from PermissionDefinitionProviders.
/// </summary>
public class AuthForgePermission
{
    public Guid Id { get; set; }

    /// <summary>
    /// Unique permission name (e.g., "Orders.Create").
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Display name for UI (e.g., "Create Order").
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// FK to the group this permission belongs to.
    /// </summary>
    public Guid GroupId { get; set; }
    public virtual AuthForgePermissionGroup Group { get; set; } = default!;

    /// <summary>
    /// FK to parent permission for hierarchy (null if root-level).
    /// </summary>
    public Guid? ParentPermissionId { get; set; }
    public virtual AuthForgePermission? ParentPermission { get; set; }

    public virtual ICollection<AuthForgePermission> Children { get; set; } = new List<AuthForgePermission>();
    public virtual ICollection<AuthForgePermissionGrant> Grants { get; set; } = new List<AuthForgePermissionGrant>();
}
