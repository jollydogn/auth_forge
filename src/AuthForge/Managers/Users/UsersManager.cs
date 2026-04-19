using System.Text.Json;
using AuthForge.Clients;
using AuthForge.Exceptions;
using AuthForge.Models.Requests.Users;
using AuthForge.Models.Responses;

namespace AuthForge.Managers.Users;

public class UsersManager : IUsersManager
{
    protected readonly IAuthForgeAdminClient AdminClient;

    public UsersManager(IAuthForgeAdminClient adminClient)
    {
        AdminClient = adminClient;
    }

    public virtual async Task<string> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var kcRequest = new
        {
            username = request.Username,
            email = request.Email,
            firstName = request.FirstName,
            lastName = request.LastName,
            enabled = request.Enabled,
            emailVerified = request.EmailVerified,
            attributes = request.Attributes
        };

        var response = await AdminClient.PostAsync("users", kcRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                throw new AuthForgeException(AuthForgeErrorCodes.UserAlreadyExists, $"Bu kullanıcı (username veya email) zaten mevcut. Detay: {content}", response.StatusCode);

            throw new AuthForgeException(AuthForgeErrorCodes.UserCreationFailed, $"Kullanıcı oluşturulamadı. Status: {response.StatusCode}, Detay: {content}", response.StatusCode);
        }

        // Location header'dan yeni oluşturulan User ID'yi çıkartırız. Link: /admin/realms/{realm}/users/{uuid}
        var locationHeader = response.Headers.Location;
        string newUserId = string.Empty;
        
        if (locationHeader != null)
        {
            var segments = locationHeader.Segments;
            newUserId = segments.Last().Trim('/');
        }

        // Eğer kullanıcı adı bulunamadıysa (location header konfigürasyon yüzünden gelmediyse) kullanıcıyı ID ile ararız
        if (string.IsNullOrEmpty(newUserId))
        {
            var user = await GetUserByUsernameAsync(request.Username, cancellationToken);
            if (user != null) newUserId = user.Id;
            else throw new AuthForgeException(AuthForgeErrorCodes.UserCreationFailed, "Kullanıcı oluşturuldu ancak ID'si alınamadı.");
        }

        // Eğer başlangıç şifresi gönderildiyse şifreyi set et
        if (!string.IsNullOrEmpty(request.InitialPassword))
        {
            await SetUserPasswordAsync(newUserId, request.InitialPassword, request.IsTemporaryPassword, cancellationToken);
        }

        return newUserId;
    }

    public virtual async Task UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Güncelleme işlemi için önce mevcut veriyi çekmek yerine Keycloak kısmi güncellemeyi (PATCH/PUT payload'unda olanları) destekler. 
        var kcRequest = new
        {
            email = request.Email,
            firstName = request.FirstName,
            lastName = request.LastName,
            enabled = request.Enabled,
            emailVerified = request.EmailVerified,
            attributes = request.Attributes
        };

        // null olan alanları serialize etmemek için JsonSerializer seçeneklerini ayarlayabiliriz veya payload hazırlayabiliriz.
        // Genellikle Keycloak sadece gönderilenleri günceller, gönderilmeyenleri ezmez.
        var jsonOptions = new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
        
        using var jsonContent = System.Net.Http.Json.JsonContent.Create(kcRequest, options: jsonOptions);
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"users/{userId}")
        {
            Content = jsonContent
        };

        var clientType = typeof(AuthForgeAdminClient); // Protected metodlara erişemediğim için IAuthForgeAdminClient'te PutAsync generic obje kabul ediyor.
        var response = await AdminClient.PutAsync($"users/{userId}", kcRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthForgeException(AuthForgeErrorCodes.UserUpdateFailed, $"Kullanıcı güncellenemedi. Status: {response.StatusCode}, Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await AdminClient.DeleteAsync($"users/{userId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new AuthForgeException(AuthForgeErrorCodes.UserNotFound, "Silinmek istenen kullanıcı bulunamadı.", response.StatusCode);

            throw new AuthForgeException(AuthForgeErrorCodes.UserDeletionFailed, $"Kullanıcı silinemedi. Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task<UserResponse> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await AdminClient.GetAsync<UserResponse>($"users/{userId}", cancellationToken);
            if (user == null) throw new Exception();
            return user;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new AuthForgeException(AuthForgeErrorCodes.UserNotFound, "Kullanıcı bulunamadı.", ex, System.Net.HttpStatusCode.NotFound);
        }
    }

    public virtual async Task<UserResponse?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var users = await AdminClient.GetAsync<List<UserResponse>>($"users?username={username}&exact=true", cancellationToken);
        return users?.FirstOrDefault();
    }

    public virtual async Task<List<UserResponse>> GetUsersAsync(string? search = null, int? first = null, int? max = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={search}");
        if (first.HasValue) queryParams.Add($"first={first}");
        if (max.HasValue) queryParams.Add($"max={max}");

        string queryStr = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

        var users = await AdminClient.GetAsync<List<UserResponse>>($"users{queryStr}", cancellationToken);
        return users ?? new List<UserResponse>();
    }

    public virtual async Task SetUserPasswordAsync(string userId, string newPassword, bool isTemporary = false, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            type = "password",
            value = newPassword,
            temporary = isTemporary
        };

        var response = await AdminClient.PutAsync($"users/{userId}/reset-password", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthForgeException(AuthForgeErrorCodes.ResetPasswordFailed, $"Şifre sıfırlama başarısız. Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task ExecuteActionsEmailAsync(string userId, List<string> actions, CancellationToken cancellationToken = default)
    {
        var response = await AdminClient.PutAsync($"users/{userId}/execute-actions-email", actions, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthForgeException(AuthForgeErrorCodes.ExternalApiError, $"Email aksiyonu tetiklenemedi. Detay: {content}", response.StatusCode);
        }
    }
}
