namespace AuthForge.Models.Requests.Auth;

/// <summary>Şifremi unuttum isteği</summary>
public class ForgotPasswordRequest
{
    /// <summary>
    /// Kullanıcının kayıtlı email adresi.
    /// Keycloak bu adrese şifre sıfırlama linki gönderir.
    /// </summary>
    public string Email { get; set; } = default!;
}
