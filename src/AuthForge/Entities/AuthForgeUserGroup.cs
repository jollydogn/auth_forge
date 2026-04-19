using System;

namespace AuthForge.Entities;

public class AuthForgeUserGroup
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }

    public virtual AuthForgeUser User { get; set; } = default!;
    public virtual AuthForgeGroup Group { get; set; } = default!;
}
