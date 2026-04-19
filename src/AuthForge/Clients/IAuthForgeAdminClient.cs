namespace AuthForge.Clients;

/// <summary>
/// Keycloak Admin REST API ile iletişim kuracak temel HTTP Client arayüzü.
/// Admin Token alınmasını ve cache'lenmesini soyutlar.
/// </summary>
public interface IAuthForgeAdminClient
{
    /// <summary>
    /// Geçerli bir Keycloak Admin Access Token döner.
    /// Süresi dolmamışsa cache'den verir, aksi takdirde yeni token alıp cache'e atar.
    /// </summary>
    Task<string> GetAdminTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>Keycloak Admin API'ye GET isteği atar ve yanıtı deserialize eder</summary>
    Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default);

    /// <summary>Keycloak Admin API'ye POST isteği atar</summary>
    Task<HttpResponseMessage> PostAsync(string url, object? data = null, CancellationToken cancellationToken = default);

    /// <summary>Keycloak Admin API'ye PUT isteği atar</summary>
    Task<HttpResponseMessage> PutAsync(string url, object? data = null, CancellationToken cancellationToken = default);

    /// <summary>Keycloak Admin API'ye DELETE isteği atar</summary>
    Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken cancellationToken = default);
}
