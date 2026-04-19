using System;
using System.Collections.Generic;

namespace AuthForge.Entities;

public class AuthForgeRole
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public virtual ICollection<AuthForgeUserRole> UserRoles { get; set; } = new List<AuthForgeUserRole>();
}
