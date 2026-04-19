namespace AuthForge.Models.Requests.Auth;

/// <summary>Refresh token ile yeni access token alma isteği</summary>
public class RefreshTokenRequest
{
    /// <summary>Mevcut refresh token değeri</summary>
    public string RefreshToken { get; set; } = default!;
}
