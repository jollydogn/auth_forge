namespace AuthForge.Models.Requests.Users;

/// <summary>Kullanıcı güncelleme isteği (Admin API)</summary>
public class UpdateUserRequest
{
    /// <summary>Yeni email adresi (opsiyonel)</summary>
    public string? Email { get; set; }

    /// <summary>Yeni ad (opsiyonel)</summary>
    public string? FirstName { get; set; }

    /// <summary>Yeni soyad (opsiyonel)</summary>
    public string? LastName { get; set; }

    /// <summary>Kullanıcı aktiflik durumu (opsiyonel)</summary>
    public bool? Enabled { get; set; }

    /// <summary>Email doğrulama durumu (opsiyonel)</summary>
    public bool? EmailVerified { get; set; }

    /// <summary>Güncellenecek attribute'lar (opsiyonel)</summary>
    public Dictionary<string, string>? Attributes { get; set; }
}
