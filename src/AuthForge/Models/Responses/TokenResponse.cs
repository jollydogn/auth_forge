using System.Text.Json.Serialization;

namespace AuthForge.Models.Responses;

/// <summary>
/// Keycloak token endpoint'inden dönen token yanıtı.
/// Login, Register (AutoLogin), Refresh Token operasyonlarında kullanılır.
/// </summary>
public class TokenResponse
{
    /// <summary>JWT Access Token. API çağrılarında Authorization: Bearer {token} olarak kullanılır.</summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;

    /// <summary>Refresh Token. Access token süresi dolduğunda yeni token almak için kullanılır.</summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>Access token'ın geçerlilik süresi (saniye)</summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>Refresh token'ın geçerlilik süresi (saniye)</summary>
    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    /// <summary>Token tipi. Genellikle "Bearer"</summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "Bearer";

    /// <summary>Token'ın kapsadığı scope'lar</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>Keycloak session state</summary>
    [JsonPropertyName("session_state")]
    public string? SessionState { get; set; }
}
