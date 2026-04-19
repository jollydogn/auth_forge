using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthForge.Models.Responses;

namespace AuthForge.Managers.Permissions;

/// <summary>
/// Fine-Grained Authorization yapısı kurmak için sistem izinlerini (Permissions) yönetir.
/// Roller (Roles) meslek/grup temsil ederken; İzinler (Permissions) doğrudan aksiyonları temsil eder (Örn: 'article-create').
/// </summary>
public interface IPermissionsManager
{
    /// <summary>
    /// Sistemde yeni bir izin tanımlar.
    /// </summary>
    Task<string?> CreatePermissionAsync(string permissionName, string description = "");

    /// <summary>
    /// Tanımlanmış bir izni doğrudan bir Kullanıcıya atar. (Sadece o adama özel istisnai yetki)
    /// </summary>
    Task<bool> AssignPermissionToUserAsync(Guid userId, string permissionName);

    /// <summary>
    /// Tanımlanmış bir izni doğrudan bir Role atar. 
    /// Böylece o Role (Örn: Admin) atanan tüm kullanıcılar aynı zamanda bu izni (Örn: 'article-create') elde eder.
    /// Keycloak içindeki Composite Roles mimarisini kullanır.
    /// </summary>
    Task<bool> AssignPermissionToRoleAsync(string roleName, string permissionName);

    /// <summary>
    /// Bir rolün sahip olduğu tüm alt izinleri (Permissions) listeler.
    /// </summary>
    Task<IEnumerable<RoleResponse>> GetPermissionsOfRoleAsync(string roleName);
}
