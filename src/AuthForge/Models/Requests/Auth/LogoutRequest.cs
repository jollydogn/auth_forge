namespace AuthForge.Models.Requests.Auth;

/// <summary>Çıkış yapma isteği. Refresh token revoke edilir.</summary>
public class LogoutRequest
{
    /// <summary>Revoke edilecek refresh token</summary>
    public string RefreshToken { get; set; } = default!;
}
