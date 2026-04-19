using System.Security.Claims;

namespace AuthForge.Services;

/// <summary>
/// Aktif kullanıcının kimlik bilgilerini (claims tabanlı) kolayca okumak için servis arayüzü.
/// </summary>
public interface ICurrentUser
{
    /// <summary>Kullanıcı oturum açmış mı?</summary>
    bool IsAuthenticated { get; }

    /// <summary>Kullanıcı ID'si (sub veya NameIdentifier claim)</summary>
    string? Id { get; }

    /// <summary>Kullanıcı adı (preferred_username veya Name claim)</summary>
    string? UserName { get; }

    /// <summary>Kullanıcının email adresi</summary>
    string? Email { get; }

    /// <summary>Kullanıcının telefon numarası</summary>
    string? PhoneNumber { get; }

    /// <summary>Kullanıcının adı (given_name)</summary>
    string? Name { get; }

    /// <summary>Kullanıcının soyadı (family_name)</summary>
    string? SurName { get; }

    /// <summary>Kullanıcının atandığı tüm roller (.NET standart Role claim'ine çevrilmiş halleri)</summary>
    string[] Roles { get; }

    /// <summary>Belirli bir claim değerini bulur</summary>
    Claim? FindClaim(string claimType);

    /// <summary>Aynı tipteki birden fazla claim'i bulur</summary>
    Claim[] FindClaims(string claimType);

    /// <summary>Tüm claimleri dizi olarak döner</summary>
    Claim[] GetAllClaims();

    /// <summary>Kullanıcının belirtilen role sahip olup olmadığını kontrol eder</summary>
    bool IsInRole(string roleName);
}
