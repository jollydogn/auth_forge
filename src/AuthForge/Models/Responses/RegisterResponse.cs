namespace AuthForge.Models.Responses;

/// <summary>Kayıt işlemi yanıtı</summary>
public class RegisterResponse
{
    /// <summary>Keycloak'ta oluşturulan kullanıcı ID'si (UUID)</summary>
    public string UserId { get; set; } = default!;

    /// <summary>
    /// AutoLogin=true ise token bilgisi döner, kullanıcı hemen oturum açar.
    /// AutoLogin=false ise null döner.
    /// </summary>
    public TokenResponse? Token { get; set; }
}
