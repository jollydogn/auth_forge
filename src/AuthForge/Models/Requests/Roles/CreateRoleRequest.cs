namespace AuthForge.Models.Requests.Roles;

/// <summary>Yeni rol oluşturma isteği (Admin API)</summary>
public class CreateRoleRequest
{
    /// <summary>Rol adı (benzersiz olmalı). Örnek: "product-manager"</summary>
    public string Name { get; set; } = default!;

    /// <summary>Rol açıklaması (opsiyonel)</summary>
    public string? Description { get; set; }
}
