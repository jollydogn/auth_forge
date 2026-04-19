namespace AuthForge.Models.Requests.Auth;

/// <summary>Kullanıcı adı ile giriş isteği (Direct Grant flow)</summary>
public class LoginByUsernameRequest
{
    /// <summary>Keycloak kullanıcı adı</summary>
    public string Username { get; set; } = default!;

    /// <summary>Kullanıcı şifresi</summary>
    public string Password { get; set; } = default!;

    /// <summary>İstenen scope'lar (opsiyonel). Örnek: "openid profile email"</summary>
    public string? Scope { get; set; }
}
