namespace AuthForge.Models.Requests.Auth;

/// <summary>Şifre sıfırlama isteği (admin aracılığıyla)</summary>
public class ResetPasswordRequest
{
    /// <summary>Yeni şifre değeri</summary>
    public string NewPassword { get; set; } = default!;

    /// <summary>
    /// Geçici şifre mi?
    /// true: Kullanıcı ilk girişte şifresini değiştirmek zorunda kalır.
    /// false: Kalıcı şifre olarak atanır.
    /// </summary>
    public bool IsTemporary { get; set; } = false;
}
