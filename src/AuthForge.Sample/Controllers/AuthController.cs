using AuthForge.Managers.Auth;
using AuthForge.Models.Requests.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthForge.Sample.Controllers;

/// <summary>
/// Projenin içinde tamamen kendi kodlarımızla (tarayıcı SDK'sı olmadan)
/// Keycloak Token alımını test edeceğimiz örnek controller.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthManager _authManager;

    public AuthController(IAuthManager authManager)
    {
        _authManager = authManager;
    }

    /// <summary>
    /// Kullanıcı adı ve şifre ile JWT Access Token + Refresh Token elde eder.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginByUsernameRequest request)
    {
        // Gelen yanıt (TokenResponse), Swagger "Authorize" butonuna yapıştırılarak sistem test edilebilir.
        var tokenResponse = await _authManager.LoginByUsernameAsync(request);
        return Ok(tokenResponse);
    }
}
