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
}
