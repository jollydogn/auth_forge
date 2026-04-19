namespace AuthForge.Configuration;

/// <summary>
/// Consumer mode için JWT Bearer yapılandırma ayarları.
/// Tüm mikroservisler bu ayarları kullanarak token doğrulaması yapar.
/// </summary>
public class AuthForgeOptions
{
    /// <summary>
    /// Keycloak realm URL.
    /// Örnek: "http://localhost:8080/realms/my-realm"
    /// .well-known endpoint'i bu URL üzerinden otomatik keşfedilir.
    /// </summary>
    public string Authority { get; set; } = default!;

    /// <summary>
    /// Token'ın hedef kitlesi (Keycloak client_id).
    /// Örnek: "my-client" veya "account"
    /// </summary>
    public string Audience { get; set; } = "account";

    /// <summary>
    /// HTTPS zorunluluğu. Production ortamında true olmalıdır.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// Keycloak realm_access.roles claim'ini ClaimTypes.Role'e dönüştür.
    /// [Authorize(Roles = "admin")] ile doğrudan çalışmasını sağlar.
    /// </summary>
    public bool MapRealmRolesToClaims { get; set; } = true;

    /// <summary>
    /// Keycloak resource_access.{client}.roles claim'ini ClaimTypes.Role'e dönüştür.
    /// </summary>
    public bool MapClientRolesToClaims { get; set; } = true;

    /// <summary>
    /// Token'daki name claim tipi.
    /// Keycloak default: "preferred_username"
    /// </summary>
    public string NameClaimType { get; set; } = "preferred_username";

    /// <summary>
    /// Token'daki role claim tipi.
    /// </summary>
    public string RoleClaimType { get; set; } = ClaimTypes.Role;

    /// <summary>
    /// Token süresinin dolmasına kaç saniye kala yenilenecek.
    /// </summary>
    public int TokenRefreshBufferSeconds { get; set; } = 30;
}
