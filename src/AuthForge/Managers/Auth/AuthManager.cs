using System.Net.Http.Json;
using System.Text.Json;
using AuthForge.Configuration;
using AuthForge.Exceptions;
using AuthForge.Models.Requests.Auth;
using AuthForge.Models.Responses;
using Microsoft.Extensions.Options;

namespace AuthForge.Managers.Auth;

/// <summary>
/// Keycloak ile iletişim kurarak Auth akışlarını yürüten ana sınıf.
/// Tüm metotlar genişletilebilir (virtual) olarak tanımlanmıştır.
/// SOLID'e uygun şekilde sadece Manager katmanındadır.
/// </summary>
public class AuthManager : IAuthManager
{
    protected readonly IHttpClientFactory HttpClientFactory;
    protected readonly AuthForgeOptions AuthOptions;
    protected readonly AuthForgeAdminOptions AdminOptions;
    protected readonly JsonSerializerOptions JsonOptions;

    public AuthManager(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthForgeOptions> authOptions,
        IOptions<AuthForgeAdminOptions> adminOptions)
    {
        HttpClientFactory = httpClientFactory;
        AuthOptions = authOptions.Value;
        AdminOptions = adminOptions.Value;
        JsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    /// <summary>Keycloak Token endpoint'ine istek atar</summary>
    protected virtual async Task<TokenResponse> RequestTokenAsync(Dictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        var client = HttpClientFactory.CreateClient("AuthForgeClient");
        
        // Token Endpoint: {BaseUrl}/realms/{Realm}/protocol/openid-connect/token
        var tokenEndpoint = $"{AdminOptions.BaseUrl.TrimEnd('/')}/realms/{AdminOptions.Realm}/protocol/openid-connect/token";

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        var response = await client.SendAsync(requestMessage, cancellationToken);
        var contentString = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new AuthForgeException(
                AuthForgeErrorCodes.TokenGenerationFailed, 
                $"Keycloak token isteği başarısız oldu. Durum: {response.StatusCode}, Detay: {contentString}",
                response.StatusCode);
        }

        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(contentString, JsonOptions);
        if (tokenResponse == null)
        {
            throw new AuthForgeException(AuthForgeErrorCodes.TokenGenerationFailed, "Token JSON deserialize edilemedi.");
        }

        return tokenResponse;
    }

    public virtual Task<TokenResponse> LoginByUsernameAsync(LoginByUsernameRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "client_id", AdminOptions.LoginClientId },
            { "grant_type", "password" },
            { "username", request.Username },
            { "password", request.Password }
        };

        if (!string.IsNullOrEmpty(AdminOptions.LoginClientSecret))
        {
            parameters.Add("client_secret", AdminOptions.LoginClientSecret);
        }

        if (!string.IsNullOrEmpty(request.Scope))
        {
            parameters.Add("scope", request.Scope);
        }

        return RequestTokenAsync(parameters, cancellationToken);
    }

    public virtual Task<TokenResponse> LoginByEmailAsync(LoginByEmailRequest request, CancellationToken cancellationToken = default)
    {
        // Keycloak varsayılan konfigürasyonda username alanına email yazılmasına da izin verir.
        var parameters = new Dictionary<string, string>
        {
            { "client_id", AdminOptions.LoginClientId },
            { "grant_type", "password" },
            { "username", request.Email },
            { "password", request.Password }
        };

        if (!string.IsNullOrEmpty(AdminOptions.LoginClientSecret))
            parameters.Add("client_secret", AdminOptions.LoginClientSecret);
            
        if (!string.IsNullOrEmpty(request.Scope))
            parameters.Add("scope", request.Scope);

        return RequestTokenAsync(parameters, cancellationToken);
    }

    public virtual Task<TokenResponse> LoginByMobileAsync(LoginByMobileRequest request, CancellationToken cancellationToken = default)
    {
        // Not: Gerçek bir "mobile" login için Keycloak'da Custom Authenticator yazılmış olmalı
        // veya kullanıcının username'i telefon numarası formatında saklanmış olmalıdır.
        // Biz burada varsayılan 'username' alanına phoneNumber'ı gönderiyoruz.
        var parameters = new Dictionary<string, string>
        {
            { "client_id", AdminOptions.LoginClientId },
            { "grant_type", "password" },
            { "username", request.PhoneNumber },
            { "password", request.Password }
        };

        if (!string.IsNullOrEmpty(AdminOptions.LoginClientSecret))
            parameters.Add("client_secret", AdminOptions.LoginClientSecret);
            
        if (!string.IsNullOrEmpty(request.Scope))
            parameters.Add("scope", request.Scope);

        return RequestTokenAsync(parameters, cancellationToken);
    }

    public virtual async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "client_id", AdminOptions.LoginClientId },
            { "grant_type", "refresh_token" },
            { "refresh_token", request.RefreshToken }
        };

        if (!string.IsNullOrEmpty(AdminOptions.LoginClientSecret))
            parameters.Add("client_secret", AdminOptions.LoginClientSecret);

        try 
        {
            return await RequestTokenAsync(parameters, cancellationToken);
        }
        catch (AuthForgeException ex)
        {
            throw new AuthForgeException(AuthForgeErrorCodes.TokenRefreshFailed, "Refresh token geçersiz veya süresi dolmuş.", ex, ex.StatusCode);
        }
    }

    public virtual async Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        var client = HttpClientFactory.CreateClient("AuthForgeClient");
        var logoutEndpoint = $"{AdminOptions.BaseUrl.TrimEnd('/')}/realms/{AdminOptions.Realm}/protocol/openid-connect/logout";

        var parameters = new Dictionary<string, string>
        {
            { "client_id", AdminOptions.LoginClientId },
            { "refresh_token", request.RefreshToken }
        };

        if (!string.IsNullOrEmpty(AdminOptions.LoginClientSecret))
            parameters.Add("client_secret", AdminOptions.LoginClientSecret);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, logoutEndpoint)
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        var response = await client.SendAsync(requestMessage, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var contentString = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthForgeException(
                AuthForgeErrorCodes.TokenRevocationFailed, 
                $"Çıkış işlemi başarısız oldu. Durum: {response.StatusCode}, Detay: {contentString}",
                response.StatusCode);
        }
    }

    public virtual Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Not: Bu kısım Admin API (veya IUserManager) tetiklenerek yapılmalı.
        // Register işleminin Keycloak REST API çağrısı, IUserManager implementasyonu içerisinde ele alınacağı için 
        // NotImplemented bırakılmış ve daha sonraki aşamalarda IUserManager'a devredileceği not edilmiştir.
        // Fakat basit public registration açıksa Keycloak endpoint'i farklıdır.
        throw new NotImplementedException("Kayıt işlemi Admin API üzerinden UsersManager vasıtasıyla kodlanacaktır.");
    }

    public virtual Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Forgot Password işlemi Admin API execute-actions-email endpoint'i ile kodlanacaktır.");
    }

    public virtual Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Reset Password işlemi UsersManager üzerinden admin context'i ile yapılacaktır.");
    }
}
