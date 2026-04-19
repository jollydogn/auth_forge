using System.Text.Json.Serialization;

namespace AuthForge.Models.Responses;

/// <summary>Keycloak'tan dönen grup bilgisi</summary>
public class GroupResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>Grup path'i. Örnek: "/engineering-team/backend"</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>Alt gruplar (nested groups)</summary>
    [JsonPropertyName("subGroups")]
    public List<GroupResponse>? SubGroups { get; set; }

    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }
}
