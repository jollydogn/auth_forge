using System;

namespace AuthForge.Entities;

public class AuthForgeUserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public virtual AuthForgeUser User { get; set; } = default!;
    public virtual AuthForgeRole Role { get; set; } = default!;
}
