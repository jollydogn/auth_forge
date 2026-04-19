using System.Text.Json.Serialization;

namespace AuthForge.Models.Responses;

/// <summary>
/// Token introspection yanıtı.
/// Token'ın geçerliliğini ve içeriğini doğrulamak için kullanılır.
/// </summary>
public class TokenIntrospectionResponse
{
    /// <summary>Token aktif mi? false ise expired veya geçersiz.</summary>
    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>Subject — Kullanıcı ID</summary>
    [JsonPropertyName("sub")]
    public string? Sub { get; set; }

    [JsonPropertyName("exp")]
    public long? Exp { get; set; }

    [JsonPropertyName("iat")]
    public long? Iat { get; set; }

    [JsonPropertyName("realm_access")]
    public RealmAccessClaims? RealmAccess { get; set; }

    [JsonPropertyName("resource_access")]
    public Dictionary<string, RealmAccessClaims>? ResourceAccess { get; set; }
}

/// <summary>Realm veya resource erişim rol listesi</summary>
public class RealmAccessClaims
{
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = new();
}
