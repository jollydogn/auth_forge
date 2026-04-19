namespace AuthForge.Models.Requests.Groups;

/// <summary>Grup güncelleme isteği</summary>
public class UpdateGroupRequest
{
    /// <summary>Yeni grup adı</summary>
    public string Name { get; set; } = default!;
}
