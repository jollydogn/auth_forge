namespace AuthForge.Models.Requests.Roles;

/// <summary>Kullanıcıya rol atama/kaldırma için kullanılan rol referansı</summary>
public class RoleRequest
{
    /// <summary>Keycloak role ID</summary>
    public string Id { get; set; } = default!;

    /// <summary>Rol adı</summary>
    public string Name { get; set; } = default!;
}
