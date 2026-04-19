using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AuthForge.Services;

/// <summary>
/// IHttpContextAccessor bağımlılığı üzerinden HttpContext.User objesini okuyarak 
/// AuthForge standart tiplerini döndüren somut ICurrentUser yapısı.
/// </summary>
public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected virtual ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public virtual bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public virtual string? Id => Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                              ?? Principal?.FindFirst("sub")?.Value;

    public virtual string? UserName => Principal?.FindFirst(ClaimTypes.Name)?.Value 
                                    ?? Principal?.FindFirst("preferred_username")?.Value;

    public virtual string? Email => Principal?.FindFirst(ClaimTypes.Email)?.Value;

    public virtual string? PhoneNumber => Principal?.FindFirst(ClaimTypes.MobilePhone)?.Value;

    public virtual string? Name => Principal?.FindFirst(ClaimTypes.GivenName)?.Value 
                                ?? Principal?.FindFirst("given_name")?.Value;

    public virtual string? SurName => Principal?.FindFirst(ClaimTypes.Surname)?.Value 
                                   ?? Principal?.FindFirst("family_name")?.Value;

    public virtual string[] Roles => FindClaims(ClaimTypes.Role).Select(c => c.Value).ToArray();

    public virtual Claim? FindClaim(string claimType)
    {
        return Principal?.FindFirst(claimType);
    }

    public virtual Claim[] FindClaims(string claimType)
    {
        return Principal?.FindAll(claimType).ToArray() ?? Array.Empty<Claim>();
    }

    public virtual Claim[] GetAllClaims()
    {
        return Principal?.Claims.ToArray() ?? Array.Empty<Claim>();
    }

    public virtual bool IsInRole(string roleName)
    {
        return Roles.Contains(roleName);
    }
}
