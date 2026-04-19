namespace AuthForge.Models.Requests.Roles;

/// <summary>Rol güncelleme isteği</summary>
public class UpdateRoleRequest
{
    /// <summary>Yeni rol adı</summary>
    public string Name { get; set; } = default!;

    /// <summary>Yeni açıklama (opsiyonel)</summary>
    public string? Description { get; set; }
}
