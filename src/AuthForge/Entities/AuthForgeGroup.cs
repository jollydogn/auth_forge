using System;
using System.Collections.Generic;

namespace AuthForge.Entities;

public class AuthForgeGroup
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Path { get; set; } = default!;

    public virtual ICollection<AuthForgeUserGroup> UserGroups { get; set; } = new List<AuthForgeUserGroup>();
}
