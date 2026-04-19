using AuthForge.Models.Requests.Roles;
using AuthForge.Models.Responses;

namespace AuthForge.Managers.Roles;

/// <summary>
/// Keycloak Realm Rolleri ve Client Rolleri Yönetimi Arayüzü
/// </summary>
public interface IRolesManager
{
    /// <summary>Yeni bir realm rolü oluşturur.</summary>
    Task CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>Rolü günceller.</summary>
    Task UpdateRoleAsync(string roleName, UpdateRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>İsme göre rolü siler.</summary>
    Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>İsme göre rol bilgilerini getirir.</summary>
    Task<RoleResponse> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>Realm içindeki tüm rolleri getirir (Arama ve sayfalama destekler).</summary>
    Task<List<RoleResponse>> GetRolesAsync(string? search = null, int? first = null, int? max = null, CancellationToken cancellationToken = default);

    /// <summary>Kullanıcıya rol atar.</summary>
    Task AssignRoleToUserAsync(string userId, RoleRequest role, CancellationToken cancellationToken = default);

    /// <summary>Kullanıcıdan rolü kaldırır.</summary>
    Task UnassignRoleFromUserAsync(string userId, RoleRequest role, CancellationToken cancellationToken = default);

    /// <summary>Kullanıcının sahip olduğu tüm realm rollerini getirir.</summary>
    Task<List<RoleResponse>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
}
