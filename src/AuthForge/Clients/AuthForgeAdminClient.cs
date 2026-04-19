using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AuthForge.Configuration;
using AuthForge.Exceptions;
using AuthForge.Models.Responses;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AuthForge.Clients;

/// <summary>
/// Keycloak Admin REST API HTTP İstemcisi.
/// Admin Token'i otomatik alır, süresi bitene kadar MemoryCache'te tutar.
/// Tüm isteklerde Authorization header'a bu token'i ekler.
/// </summary>
public class AuthForgeAdminClient : IAuthForgeAdminClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthForgeAdminOptions _adminOptions;
    private readonly IMemoryCache _memoryCache;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string AdminTokenCacheKey = "AuthForge_Keycloak_AdminToken";

    public AuthForgeAdminClient(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthForgeAdminOptions> adminOptions,
        IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _adminOptions = adminOptions.Value;
        _memoryCache = memoryCache;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public virtual async Task<string> GetAdminTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue(AdminTokenCacheKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
        {
            return cachedToken;
        }

        var client = _httpClientFactory.CreateClient("AuthForgeAdminClient");
        var tokenEndpoint = $"{_adminOptions.BaseUrl.TrimEnd('/')}/realms/{_adminOptions.AdminRealm}/protocol/openid-connect/token";

        var parameters = new Dictionary<string, string>
        {
            { "client_id", _adminOptions.ClientId },
            { "client_secret", _adminOptions.ClientSecret },
            { "grant_type", "client_credentials" }
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        var response = await client.SendAsync(requestMessage, cancellationToken);
        var contentString = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new AuthForgeException(
                AuthForgeErrorCodes.TokenGenerationFailed,
                $"Admin Token alınamadı. Configuration'ı kontrol edin. {response.StatusCode}: {contentString}",
                response.StatusCode);
        }

        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(contentString, _jsonOptions);
        if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            throw new AuthForgeException(AuthForgeErrorCodes.TokenGenerationFailed, "Admin Token cevabı boş.");
        }

        // Token cache'leme. Token'ın tam ölüm süresinden biraz önce yenilemesi için Cache duration'ı options'dan alır.
        _memoryCache.Set(
            AdminTokenCacheKey, 
            tokenResponse.AccessToken, 
            TimeSpan.FromSeconds(_adminOptions.TokenCacheDurationSeconds));

        return tokenResponse.AccessToken;
    }

    private async Task<HttpClient> CreateConfiguredClientAsync(CancellationToken cancellationToken)
    {
        var token = await GetAdminTokenAsync(cancellationToken);
        var client = _httpClientFactory.CreateClient("AuthForgeAdminClient");
        // Keycloak Admin REST API ana basePath: /admin/realms/{Realm}
        client.BaseAddress = new Uri($"{_adminOptions.BaseUrl.TrimEnd('/')}/admin/realms/{_adminOptions.Realm}/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public virtual async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        var client = await CreateConfiguredClientAsync(cancellationToken);
        var response = await client.GetAsync(url, cancellationToken);
        
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);
    }

    public virtual async Task<HttpResponseMessage> PostAsync(string url, object? data = null, CancellationToken cancellationToken = default)
    {
        var client = await CreateConfiguredClientAsync(cancellationToken);
        
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        if (data != null)
        {
            request.Content = JsonContent.Create(data, options: _jsonOptions);
        }

        return await client.SendAsync(request, cancellationToken);
    }

    public virtual async Task<HttpResponseMessage> PutAsync(string url, object? data = null, CancellationToken cancellationToken = default)
    {
        var client = await CreateConfiguredClientAsync(cancellationToken);
        
        var request = new HttpRequestMessage(HttpMethod.Put, url);
        if (data != null)
        {
            request.Content = JsonContent.Create(data, options: _jsonOptions);
        }

        return await client.SendAsync(request, cancellationToken);
    }

    public virtual async Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var client = await CreateConfiguredClientAsync(cancellationToken);
        return await client.DeleteAsync(url, cancellationToken);
    }
}
