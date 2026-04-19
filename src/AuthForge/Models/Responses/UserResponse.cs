using System.Text.Json.Serialization;

namespace AuthForge.Models.Responses;

/// <summary>Keycloak'tan dönen kullanıcı bilgisi (Admin REST API)</summary>
public class UserResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("username")]
    public string Username { get; set; } = default!;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; }

    /// <summary>Oluşturulma tarihi (Unix milisaniye)</summary>
    [JsonPropertyName("createdTimestamp")]
    public long CreatedTimestamp { get; set; }

    /// <summary>Oluşturulma tarihi UTC olarak</summary>
    public DateTime CreatedAt => DateTimeOffset.FromUnixTimeMilliseconds(CreatedTimestamp).UtcDateTime;

    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }
}
