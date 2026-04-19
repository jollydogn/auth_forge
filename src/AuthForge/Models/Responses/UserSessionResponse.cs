using System.Text.Json.Serialization;

namespace AuthForge.Models.Responses;

/// <summary>Kullanıcı aktif oturum bilgisi</summary>
public class UserSessionResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("ipAddress")]
    public string? IpAddress { get; set; }

    /// <summary>Oturum başlangıç zamanı (Unix timestamp saniye)</summary>
    [JsonPropertyName("start")]
    public long Start { get; set; }

    /// <summary>Son erişim zamanı (Unix timestamp saniye)</summary>
    [JsonPropertyName("lastAccess")]
    public long LastAccess { get; set; }

    public DateTime StartedAt => DateTimeOffset.FromUnixTimeSeconds(Start).UtcDateTime;
    public DateTime LastAccessedAt => DateTimeOffset.FromUnixTimeSeconds(LastAccess).UtcDateTime;

    [JsonPropertyName("clients")]
    public Dictionary<string, string>? Clients { get; set; }
}
