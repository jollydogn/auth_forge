using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthForge.Clients;
using AuthForge.Configuration;
using AuthForge.Exceptions;
using AuthForge.Models.Requests.Roles;
using AuthForge.Models.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthForge.Managers.Permissions;

public class PermissionsManager : IPermissionsManager
{
    private readonly IAuthForgeAdminClient _adminClient;
    private readonly AuthForgeAdminOptions _options;
    private readonly ILogger<PermissionsManager> _logger;

    public PermissionsManager(
        IAuthForgeAdminClient adminClient,
        IOptionsSnapshot<AuthForgeAdminOptions> options,
        ILogger<PermissionsManager> logger)
    {
        _adminClient = adminClient;
        _options = options.Value;
        _logger = logger;
    }

    public virtual async Task<string?> CreatePermissionAsync(string permissionName, string description = "")
    {
        var endpoint = $"admin/realms/{_options.Realm}/roles";
        
        var request = new CreateRoleRequest
        {
            Name = permissionName,
            Description = $"[Permission] {description}"
        };

        try
        {
            await _adminClient.PostAsync(endpoint, request);
            _logger.LogInformation("Permission created successfully: {Permission}", permissionName);
            
            // Get ID
            var roleResponse = await _adminClient.GetAsync<RoleResponse>($"admin/realms/{_options.Realm}/roles/{permissionName}");
            return roleResponse?.Id.ToString();
        }
        catch (AuthForgeException ex) when (ex.Message.Contains("409"))
        {
            _logger.LogWarning("Permission already exists: {Permission}", permissionName);
            return null;
        }
    }

    public virtual async Task<bool> AssignPermissionToUserAsync(Guid userId, string permissionName)
    {
        // 1. Permission (Rol) detaylarını al ki ID vb gelsin
        var permissionResponse = await _adminClient.GetAsync<RoleResponse>($"admin/realms/{_options.Realm}/roles/{permissionName}");
        if (permissionResponse == null) 
            throw new Exception("Permission not found in Keycloak.");

        // 2. Kullanıcıya atama yap
        var endpoint = $"admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm";
        await _adminClient.PostAsync(endpoint, new List<object>
        {
            new { id = permissionResponse.Id, name = permissionResponse.Name }
        });

        _logger.LogInformation("Permission '{Permission}' assigned to user {UserId}", permissionName, userId);
        return true;
    }

    public virtual async Task<bool> AssignPermissionToRoleAsync(string roleName, string permissionName)
    {
        // 1. Hedef rolün detaylarını al (Composite Parent Role)
        var parentRoleResponse = await _adminClient.GetAsync<RoleResponse>($"admin/realms/{_options.Realm}/roles/{roleName}");
        
        // 2. İzin detaylarını al (Child Role)
        var permissionResponse = await _adminClient.GetAsync<RoleResponse>($"admin/realms/{_options.Realm}/roles/{permissionName}");

        if (parentRoleResponse == null || permissionResponse == null)
            throw new Exception("Role or Permission not found in Keycloak.");

        // 3. Composite eklemesi yap
        var endpoint = $"admin/realms/{_options.Realm}/roles/{roleName}/composites";
        await _adminClient.PostAsync(endpoint, new List<object>
        {
            new { id = permissionResponse.Id, name = permissionResponse.Name }
        });

        _logger.LogInformation("Permission '{Permission}' attached to Role '{Role}'", permissionName, roleName);
        return true;
    }

    public virtual async Task<IEnumerable<RoleResponse>> GetPermissionsOfRoleAsync(string roleName)
    {
        var endpoint = $"admin/realms/{_options.Realm}/roles/{roleName}/composites";
        return await _adminClient.GetAsync<IEnumerable<RoleResponse>>(endpoint) ?? Enumerable.Empty<RoleResponse>();
    }
}
