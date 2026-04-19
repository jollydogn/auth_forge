namespace AuthForge.Configuration;

/// <summary>
/// Admin mode için Keycloak REST API bağlantı ve kimlik doğrulama ayarları.
/// Merkezi auth servisi bu ayarları kullanarak Keycloak Admin API'si ile iletişim kurar.
/// </summary>
public class AuthForgeAdminOptions
{
    /// <summary>
    /// Keycloak base URL.
    /// Örnek: "http://localhost:8080"
    /// </summary>
    public string BaseUrl { get; set; } = default!;

    /// <summary>
    /// Yönetilecek realm adı.
    /// Örnek: "my-realm"
    /// </summary>
    public string Realm { get; set; } = "master";

    /// <summary>
    /// Admin token'ının alınacağı realm.
    /// Genellikle "master" realm kullanılır.
    /// </summary>
    public string AdminRealm { get; set; } = "master";

    /// <summary>
    /// Admin işlemler için Client ID.
    /// Bu client Confidential + Service Account + admin rolünde olmalıdır.
    /// </summary>
    public string ClientId { get; set; } = default!;

    /// <summary>
    /// Admin client secret değeri.
    /// </summary>
    public string ClientSecret { get; set; } = default!;

    /// <summary>
    /// Login (Direct Grant / Authorization Code) işlemlerinde kullanılacak Client ID.
    /// "Direct access grants" özelliği etkin olmalıdır.
    /// </summary>
    public string LoginClientId { get; set; } = default!;

    /// <summary>
    /// Login client'ın secret değeri.
    /// Public client ise null bırakılabilir.
    /// </summary>
    public string? LoginClientSecret { get; set; }

    /// <summary>
    /// Admin token cache süresi (saniye).
    /// Token bu süreden önce expire olmadan cache'den kullanılır.
    /// </summary>
    public int TokenCacheDurationSeconds { get; set; } = 50;

    /// <summary>
    /// Social login (Google vb.) sonrası yönlendirilecek redirect URI.
    /// Frontend callback sayfası.
    /// </summary>
    public string? SocialLoginRedirectUri { get; set; }
}
