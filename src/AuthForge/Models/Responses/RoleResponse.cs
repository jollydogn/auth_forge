using System.Text.Json.Serialization;

namespace AuthForge.Models.Responses;

/// <summary>Keycloak'tan dönen rol bilgisi</summary>
public class RoleResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    /// <summary>Rol adı. Örnek: "admin", "user", "moderator", "super-admin"</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Composite rol mu? (Başka rolleri içeren üst rol)</summary>
    [JsonPropertyName("composite")]
    public bool Composite { get; set; }

    /// <summary>Client rolü mü? false ise realm rolüdür.</summary>
    [JsonPropertyName("clientRole")]
    public bool ClientRole { get; set; }

    [JsonPropertyName("containerId")]
    public string? ContainerId { get; set; }
}
