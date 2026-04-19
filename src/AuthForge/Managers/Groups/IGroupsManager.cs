using AuthForge.Models.Requests.Groups;
using AuthForge.Models.Responses;

namespace AuthForge.Managers.Groups;

/// <summary>
/// Keycloak Grup Yönetimi Arayüzü
/// </summary>
public interface IGroupsManager
{
    Task CreateGroupAsync(CreateGroupRequest request, CancellationToken cancellationToken = default);
    Task UpdateGroupAsync(string groupId, UpdateGroupRequest request, CancellationToken cancellationToken = default);
    Task DeleteGroupAsync(string groupId, CancellationToken cancellationToken = default);
    Task<GroupResponse> GetGroupByIdAsync(string groupId, CancellationToken cancellationToken = default);
    Task<List<GroupResponse>> GetGroupsAsync(string? search = null, int? first = null, int? max = null, CancellationToken cancellationToken = default);
    
    Task AddUserToGroupAsync(string userId, string groupId, CancellationToken cancellationToken = default);
    Task RemoveUserFromGroupAsync(string userId, string groupId, CancellationToken cancellationToken = default);
    Task<List<GroupResponse>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default);
}
