using AuthForge.Models.Requests.Auth;
using AuthForge.Models.Responses;

namespace AuthForge.Managers.Auth;

/// <summary>
/// Kimlik doğrulama işlemleri arayüzü (Login, Token, Register, vb.)
/// </summary>
public interface IAuthManager
{
    /// <summary>Kullanıcı adı ile Token alır (Direct Access Grants)</summary>
    Task<TokenResponse> LoginByUsernameAsync(LoginByUsernameRequest request, CancellationToken cancellationToken = default);

    /// <summary>Email ile Token alır (Direct Access Grants)</summary>
    Task<TokenResponse> LoginByEmailAsync(LoginByEmailRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Telefon numarası ile giriş. Keycloak'da numaradan kullanıcıyı bulup/veya custom akış kullanılarak çalıştırılır.
    /// </summary>
    Task<TokenResponse> LoginByMobileAsync(LoginByMobileRequest request, CancellationToken cancellationToken = default);

    /// <summary>Sisteme yeni kullanıcı kaydeder (Admin API gerektirir)</summary>
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>Email adresine sıfırlama linki gönderir (Admin API üzerinden tetiklenir)</summary>
    Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>Kullanıcının şifresini Admin hakkıyla sıfırlar</summary>
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>Refresh token kullanarak yeni Access token alır</summary>
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>Kullanıcı oturumunu sonlandırır (Refresh token revoke edilir)</summary>
    Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);
}
