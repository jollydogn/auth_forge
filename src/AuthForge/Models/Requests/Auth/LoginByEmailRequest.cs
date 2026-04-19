namespace AuthForge.Models.Requests.Auth;

/// <summary>Email adresi ile giriş isteği (Direct Grant flow)</summary>
public class LoginByEmailRequest
{
    /// <summary>Kullanıcının email adresi</summary>
    public string Email { get; set; } = default!;

    /// <summary>Kullanıcı şifresi</summary>
    public string Password { get; set; } = default!;

    /// <summary>İstenen scope'lar (opsiyonel). Örnek: "openid profile email"</summary>
    public string? Scope { get; set; }
}
