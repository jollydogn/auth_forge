using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using AuthForge.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AuthForge.Authentication;

/// <summary>
/// Keycloak JWT'si içerisindeki komplex role yapılarını (realm_access, resource_access)
/// standart .NET ClaimTypes.Role formatına dönüştüren mekanizma.
/// Böylece [Authorize(Roles = "admin")] standart olarak sorunsuz çalışır.
/// </summary>
public class KeycloakClaimsTransformation : IClaimsTransformation
{
    private readonly AuthForgeOptions _options;

    public KeycloakClaimsTransformation(IOptions<AuthForgeOptions> options)
    {
        _options = options.Value;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null || !identity.IsAuthenticated)
        {
            return Task.FromResult(principal);
        }

        // 1. Name Claim Ataması (preferred_username -> NameClaimType)
        if (!string.IsNullOrEmpty(_options.NameClaimType) && !identity.HasClaim(c => c.Type == _options.NameClaimType))
        {
            var preferredUsername = principal.FindFirst("preferred_username")?.Value;
            if (!string.IsNullOrEmpty(preferredUsername))
            {
                identity.AddClaim(new Claim(_options.NameClaimType, preferredUsername));
            }
        }

        // 2. Realm Rolleri Claim Dönüşümü
        if (_options.MapRealmRolesToClaims)
        {
            var realmAccessClaim = principal.FindFirst("realm_access");
            if (realmAccessClaim != null && !string.IsNullOrEmpty(realmAccessClaim.Value))
            {
                try
                {
                    var realmAccess = JsonSerializer.Deserialize<KeycloakAccessMap>(realmAccessClaim.Value);
                    if (realmAccess?.Roles != null)
                    {
                        foreach (var role in realmAccess.Roles)
                        {
                            if (!identity.HasClaim(_options.RoleClaimType, role))
                            {
                                identity.AddClaim(new Claim(_options.RoleClaimType, role));
                            }
                        }
                    }
                }
                catch (JsonException)
                {
                    // JSON formatı hatalıysa, parsing hatasını ignore et.
                }
            }
        }

        // 3. Client Rolleri Claim Dönüşümü (Audience veya tüm clientlar için)
        if (_options.MapClientRolesToClaims)
        {
            var resourceAccessClaim = principal.FindFirst("resource_access");
            if (resourceAccessClaim != null && !string.IsNullOrEmpty(resourceAccessClaim.Value))
            {
                try
                {
                    var resourceAccess = JsonSerializer.Deserialize<Dictionary<string, KeycloakAccessMap>>(resourceAccessClaim.Value);
                    if (resourceAccess != null)
                    {
                        foreach (var client in resourceAccess)
                        {
                            if (client.Value.Roles != null)
                            {
                                foreach (var role in client.Value.Roles)
                                {
                                    // Örnek: "account-manage-account" formatında ekleyebiliriz 
                                    // Veya doğrudan rol ismini de ekleyebiliriz. Biz doğrudan ekliyoruz.
                                    var claimValue = $"{client.Key}:{role}";
                                    if (!identity.HasClaim(_options.RoleClaimType, claimValue))
                                    {
                                        // "myclient:myrole" formatında ekler. Eğer sadece rolü istersen claimValue yerine role yazabilirsin.
                                        // Fakat çakışmaları engellemek adına prefix önerilir.
                                        identity.AddClaim(new Claim(_options.RoleClaimType, claimValue));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (JsonException)
                {
                    // JSON parsing error ignore
                }
            }
        }

        return Task.FromResult(principal);
    }

    private class KeycloakAccessMap
    {
        [JsonPropertyName("roles")]
        public List<string>? Roles { get; set; }
    }
}
