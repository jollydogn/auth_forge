using Microsoft.AspNetCore.Authorization;

namespace AuthForge.Authorization;

/// <summary>
/// AuthForge modülüne özel Authorize Attribute.
/// Standart AuthorizeAttribute yeteneklerini kullanır, 
/// ancak modül içerisindeki endpoint'leri markalamak için kullanılması tavsiye edilir.
/// Örnek: [AuthForgeAuthorize(Roles = "admin, editor")]
/// </summary>
public class AuthForgeAuthorizeAttribute : AuthorizeAttribute
{
    public AuthForgeAuthorizeAttribute() 
    { 
    }

    public AuthForgeAuthorizeAttribute(string policy) : base(policy) 
    { 
    }

    /// <summary>
    /// Fine-grained Yetkilendirme (Permissions) için Semantic yapay alias.
    /// Keycloak Composite Role mimarisi sayesinde 'Permissions = "article-create"' ile 'Roles = "article-create"' teknik olarak aynı işi yapar.
    /// Consumer'ların [AuthForgeAuthorize(Permissions="...")] diyebilmesi için eklenmiştir.
    /// </summary>
    public string Permissions
    {
        get => base.Roles ?? string.Empty;
        set => base.Roles = value;
    }
}
