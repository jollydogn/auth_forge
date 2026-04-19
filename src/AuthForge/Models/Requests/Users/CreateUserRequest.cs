namespace AuthForge.Models.Requests.Users;

/// <summary>Yeni kullanıcı oluşturma isteği (Admin API)</summary>
public class CreateUserRequest
{
    /// <summary>Kullanıcı adı (benzersiz olmalı)</summary>
    public string Username { get; set; } = default!;

    /// <summary>Email adresi</summary>
    public string Email { get; set; } = default!;

    /// <summary>Ad</summary>
    public string? FirstName { get; set; }

    /// <summary>Soyad</summary>
    public string? LastName { get; set; }

    /// <summary>Kullanıcı aktif mi?</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Email doğrulandı mı?</summary>
    public bool EmailVerified { get; set; } = false;

    /// <summary>Başlangıç şifresi (opsiyonel)</summary>
    public string? InitialPassword { get; set; }

    /// <summary>Başlangıç şifresi geçici mi?</summary>
    public bool IsTemporaryPassword { get; set; } = false;

    /// <summary>Ek attribute'lar</summary>
    public Dictionary<string, string>? Attributes { get; set; }
}
