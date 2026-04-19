namespace AuthForge.Models.Requests.Groups;

/// <summary>Yeni grup oluşturma isteği (Admin API)</summary>
public class CreateGroupRequest
{
    /// <summary>Grup adı (realm içinde benzersiz olmalı). Örnek: "engineering-team"</summary>
    public string Name { get; set; } = default!;
}
