using AuthForge.Models.Requests.Users;
using AuthForge.Models.Responses;

namespace AuthForge.Managers.Users;

/// <summary>
/// Kullanıcı yönetimi arayüzü (CRUD, Şifre İşlemleri vb.)
/// </summary>
public interface IUsersManager
{
    /// <summary>Yeni kullanıcı oluşturur. Başarılı olursa kullanıcının Keycloak ID'sini (UUID) döner.</summary>
    Task<string> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>Kullanıcı bilgilerini günceller.</summary>
    Task UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>Kullanıcıyı siler.</summary>
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>ID ile kullanıcı bilgilerini getirir.</summary>
    Task<UserResponse> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>Username veya Email ile kullanıcı getirir.</summary>
    Task<UserResponse?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>Kullanıcı listesini getirir. Parametrelere göre paginasyon ve arama yapılabilir.</summary>
    Task<List<UserResponse>> GetUsersAsync(string? search = null, int? first = null, int? max = null, CancellationToken cancellationToken = default);

    /// <summary>Kullanıcının şifresini admin hakkıyla doğrudan değiştirir.</summary>
    Task SetUserPasswordAsync(string userId, string newPassword, bool isTemporary = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kullanıcıya belirtilen eylemleri yapması için email gönderir.
    /// Actions: "VERIFY_EMAIL", "UPDATE_PROFILE", "CONFIGURE_TOTP", "UPDATE_PASSWORD", "TERMS_AND_CONDITIONS"
    /// </summary>
    Task ExecuteActionsEmailAsync(string userId, List<string> actions, CancellationToken cancellationToken = default);
}
