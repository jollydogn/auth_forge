namespace AuthForge.Models.Requests.Auth;

/// <summary>Mobil cihaz bilgisi ile giriş isteği (Direct Grant flow)</summary>
public class LoginByMobileRequest
{
    /// <summary>Telefon numarası. Keycloak'da phone-number attribute olarak saklanır.</summary>
    public string PhoneNumber { get; set; } = default!;

    /// <summary>Kullanıcı şifresi</summary>
    public string Password { get; set; } = default!;

    /// <summary>Cihaz ID'si (opsiyonel, loglama/audit için)</summary>
    public string? DeviceId { get; set; }

    /// <summary>Cihaz tipi (opsiyonel). Örnek: "iOS", "Android"</summary>
    public string? DeviceType { get; set; }

    /// <summary>İstenen scope'lar (opsiyonel)</summary>
    public string? Scope { get; set; }
}
