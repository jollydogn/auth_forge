using AuthForge.Authorization;
using AuthForge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthForge.Sample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ICurrentUser _currentUser;

    public TestController(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    /// <summary>
    /// Bu endpoint'e kimlik doğrulaması olmadan da girilebilir.
    /// </summary>
    [HttpGet("public")]
    public IActionResult GetPublic()
    {
        return Ok(new 
        { 
            message = "Bu API herkese açıktır. Hoşgeldiniz!",
            time = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Geçerli bir JWT token gönderildiğinde (herhangi bir rol olsa da olmasa da) çalışır.
    /// </summary>
    [HttpGet("authenticated")]
    [Authorize] // Standart .NET Authorize attribütüdür eklentimizle entegre çalışır.
    public IActionResult GetAuthenticated()
    {
        return Ok(new 
        { 
            message = "Mükemmel! JWT Token'ınız başarıyla çözüldü.",
            user = new 
            {
                _currentUser.Id,
                _currentUser.UserName,
                _currentUser.Email,
                _currentUser.Name,
                _currentUser.SurName,
                Roles = _currentUser.Roles // Keycloak'taki realm roleri, claim'e işlenmiş olarak burada yansır
            }
        });
    }

    /// <summary>
    /// Keycloak veritabanında 'admin' rolüne sahip olan hesaplar erişebilir.
    /// </summary>
    [HttpGet("admin-only")]
    [AuthForgeAuthorize(Roles = "admin")]
    public IActionResult GetAdminOnly()
    {
        return Ok(new 
        { 
            message = $"Tebrikler {_currentUser.UserName}! Süper yetkili admin alanındasın.",
            action = "Gizli raporlara erişim sağlandı."
        });
    }

    /// <summary>
    /// Hem 'admin' hem de 'product-manager' rolüne sahip kullanıcılar işlem yapabilir.
    /// </summary>
    [HttpPost("create-product")]
    [AuthForgeAuthorize(Roles = "admin, product-manager")]
    public IActionResult CreateProduct()
    {
        return Ok(new 
        { 
            message = "Yeni ürün başarıyla eklendi.",
            createdBy = _currentUser.UserName
        });
    }
}
