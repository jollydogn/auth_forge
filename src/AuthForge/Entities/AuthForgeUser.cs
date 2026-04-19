using System;
using System.Collections.Generic;

namespace AuthForge.Entities;

/// <summary>
/// Local synchronized Keycloak user.
/// Provides a reference for LINQ queries and Joins in the local database.
/// </summary>
public class AuthForgeUser
{
    public Guid Id { get; set; } // Keycloak ID'si ile birebir eşleşir.
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool EmailVerified { get; set; }
    public bool Enabled { get; set; }
    public DateTime CreatedTimestamp { get; set; }

    public virtual ICollection<AuthForgeUserRole> UserRoles { get; set; } = new List<AuthForgeUserRole>();
    public virtual ICollection<AuthForgeUserGroup> UserGroups { get; set; } = new List<AuthForgeUserGroup>();
}
