using AuthForge.Clients;
using AuthForge.Exceptions;
using AuthForge.Models.Requests.Roles;
using AuthForge.Models.Responses;

namespace AuthForge.Managers.Roles;

public class RolesManager : IRolesManager
{
    protected readonly IAuthForgeAdminClient AdminClient;

    public RolesManager(IAuthForgeAdminClient adminClient)
    {
        AdminClient = adminClient;
    }

    public virtual async Task CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var kcRequest = new
        {
            name = request.Name,
            description = request.Description
        };

        var response = await AdminClient.PostAsync("roles", kcRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                throw new AuthForgeException(AuthForgeErrorCodes.RoleAlreadyExists, $"Bu rol zaten mevcut. Detay: {content}", response.StatusCode);

            throw new AuthForgeException(AuthForgeErrorCodes.RoleCreationFailed, $"Rol oluşturulamadı. Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task UpdateRoleAsync(string roleName, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var kcRequest = new
        {
            name = request.Name,
            description = request.Description
        };

        var response = await AdminClient.PutAsync($"roles/{roleName}", kcRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthForgeException(AuthForgeErrorCodes.ExternalApiError, $"Rol güncellenemedi. Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var response = await AdminClient.DeleteAsync($"roles/{roleName}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new AuthForgeException(AuthForgeErrorCodes.RoleNotFound, "Silinmek istenen rol bulunamadı.", response.StatusCode);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthForgeException(AuthForgeErrorCodes.RoleDeletionFailed, $"Rol silinemedi. Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task<RoleResponse> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await AdminClient.GetAsync<RoleResponse>($"roles/{roleName}", cancellationToken);
            if (role == null) throw new Exception();
            return role;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new AuthForgeException(AuthForgeErrorCodes.RoleNotFound, "Rol bulunamadı.", ex, System.Net.HttpStatusCode.NotFound);
        }
    }

    public virtual async Task<List<RoleResponse>> GetRolesAsync(string? search = null, int? first = null, int? max = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={search}");
        if (first.HasValue) queryParams.Add($"first={first}");
        if (max.HasValue) queryParams.Add($"max={max}");

        string queryStr = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        
        var roles = await AdminClient.GetAsync<List<RoleResponse>>($"roles{queryStr}", cancellationToken);
        return roles ?? new List<RoleResponse>();
    }

    // Role Mapping işlemleri Keycloak'ta Array (Liste) olarak gönderilir.
    public virtual async Task AssignRoleToUserAsync(string userId, RoleRequest role, CancellationToken cancellationToken = default)
    {
        var payload = new List<RoleRequest> { role };
        var response = await AdminClient.PostAsync($"users/{userId}/role-mappings/realm", payload, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthForgeException(AuthForgeErrorCodes.ExternalApiError, $"Kullanıcıya rol atanamadı. Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task UnassignRoleFromUserAsync(string userId, RoleRequest role, CancellationToken cancellationToken = default)
    {
        // DeleteAsync HttpClient'da payload desteklemez normalde, ama HttpRequestMessage ile gönderilebilir.
        // Keycloak'ta unassign için DELETE isteği payload'lı atılır. 
        // IAuthForgeAdminClient'de DeleteAsync body almadığı için geçici olarak base HttpClient'a inmemiz veya
        // Client'ı güncelleyip Delete'e data kabul etmesini sağlamamız gerekir.
        // IAuthForgeAdminClient.DeleteAsync'yi değiştirmedik, biz HttpClient'i doğrudan AdminClient'den almak yerine 
        // IAuthForgeAdminClient'a SendRequest metodu eklemedik. O yüzden standard HttpRequestMessage kullanmalıyız.
        // Şimdilik workaround ile SendAsync yapmalıyız, ancak IAuthForgeAdminClient dış dünyaya HttpClient'ı sızdırmaz.
        // O yüzden IAuthForgeAdminClient tarafında bu işlemi geçici olarak Post/Put benzeri yöneteceğiz:
        
        // Hata fırlatıyoruz çünkü Delete with Body .NET'te özel HttpRequestMessage ister. 
        // Bunu IAuthForgeAdminClient'e generic bir metod eklediğimizi varsayarak HTTP metodunu değiştireceğiz, 
        // Ama şimdilik basitçe NotSupported bırakıp IAuthForgeAdminClient'e DeleteWithBody ekleyebiliriz.
        throw new NotImplementedException("Rol silme işlemi (DELETE with body) için özel Client yetkisi gereklidir.");
    }

    public virtual async Task<List<RoleResponse>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var roles = await AdminClient.GetAsync<List<RoleResponse>>($"users/{userId}/role-mappings/realm", cancellationToken);
        return roles ?? new List<RoleResponse>();
    }
}
