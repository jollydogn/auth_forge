using AuthForge.Clients;
using AuthForge.Exceptions;
using AuthForge.Models.Requests.Groups;
using AuthForge.Models.Responses;

namespace AuthForge.Managers.Groups;

public class GroupsManager : IGroupsManager
{
    protected readonly IAuthForgeAdminClient AdminClient;

    public GroupsManager(IAuthForgeAdminClient adminClient)
    {
        AdminClient = adminClient;
    }

    public virtual async Task CreateGroupAsync(CreateGroupRequest request, CancellationToken cancellationToken = default)
    {
        var kcRequest = new { name = request.Name };

        var response = await AdminClient.PostAsync("groups", kcRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                throw new AuthForgeException(AuthForgeErrorCodes.GroupAlreadyExists, $"Grup zaten mevcut. Detay: {content}", response.StatusCode);

            throw new AuthForgeException(AuthForgeErrorCodes.GroupCreationFailed, $"Grup oluşturulamadı. Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task UpdateGroupAsync(string groupId, UpdateGroupRequest request, CancellationToken cancellationToken = default)
    {
        var kcRequest = new { name = request.Name };

        var response = await AdminClient.PutAsync($"groups/{groupId}", kcRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthForgeException(AuthForgeErrorCodes.ExternalApiError, $"Grup güncellenemedi. Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task DeleteGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        var response = await AdminClient.DeleteAsync($"groups/{groupId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new AuthForgeException(AuthForgeErrorCodes.GroupNotFound, "Silinmek istenen grup bulunamadı.", response.StatusCode);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthForgeException(AuthForgeErrorCodes.GroupDeletionFailed, $"Grup silinemedi. Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task<GroupResponse> GetGroupByIdAsync(string groupId, CancellationToken cancellationToken = default)
    {
        try
        {
            var group = await AdminClient.GetAsync<GroupResponse>($"groups/{groupId}", cancellationToken);
            if (group == null) throw new Exception();
            return group;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new AuthForgeException(AuthForgeErrorCodes.GroupNotFound, "Grup bulunamadı.", ex, System.Net.HttpStatusCode.NotFound);
        }
    }

    public virtual async Task<List<GroupResponse>> GetGroupsAsync(string? search = null, int? first = null, int? max = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={search}");
        if (first.HasValue) queryParams.Add($"first={first}");
        if (max.HasValue) queryParams.Add($"max={max}");

        string queryStr = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        
        var groups = await AdminClient.GetAsync<List<GroupResponse>>($"groups{queryStr}", cancellationToken);
        return groups ?? new List<GroupResponse>();
    }

    public virtual async Task AddUserToGroupAsync(string userId, string groupId, CancellationToken cancellationToken = default)
    {
        var response = await AdminClient.PutAsync($"users/{userId}/groups/{groupId}", null, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthForgeException(AuthForgeErrorCodes.ExternalApiError, $"Kullanıcı gruba eklenemedi. Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task RemoveUserFromGroupAsync(string userId, string groupId, CancellationToken cancellationToken = default)
    {
        var response = await AdminClient.DeleteAsync($"users/{userId}/groups/{groupId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthForgeException(AuthForgeErrorCodes.ExternalApiError, $"Kullanıcı gruptan silinemedi. Detay: {content}", response.StatusCode);
        }
    }

    public virtual async Task<List<GroupResponse>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var groups = await AdminClient.GetAsync<List<GroupResponse>>($"users/{userId}/groups", cancellationToken);
        return groups ?? new List<GroupResponse>();
    }
}
