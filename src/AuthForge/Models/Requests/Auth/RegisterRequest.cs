namespace AuthForge.Models.Requests.Auth;

/// <summary>Yeni kullanıcı kayıt isteği</summary>
public class RegisterRequest
{
    /// <summary>Kullanıcı adı (benzersiz olmalı)</summary>
    public string Username { get; set; } = default!;

    /// <summary>Email adresi</summary>
    public string Email { get; set; } = default!;

    /// <summary>Şifre</summary>
    public string Password { get; set; } = default!;

    /// <summary>Ad (opsiyonel)</summary>
    public string? FirstName { get; set; }

    /// <summary>Soyad (opsiyonel)</summary>
    public string? LastName { get; set; }

    /// <summary>Email doğrulandı mı? false ise verification email gönderilir.</summary>
    public bool EmailVerified { get; set; } = false;

    /// <summary>
    /// Kayıt sonrası otomatik giriş yapılsın mı?
    /// true: TokenResponse da döner, kullanıcı hemen oturum açar.
    /// </summary>
    public bool AutoLogin { get; set; } = true;

    /// <summary>Telefon numarası (opsiyonel, Keycloak attribute olarak saklanır)</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Ek kullanıcı attribute'ları (Keycloak custom attributes)</summary>
    public Dictionary<string, string>? Attributes { get; set; }
}
