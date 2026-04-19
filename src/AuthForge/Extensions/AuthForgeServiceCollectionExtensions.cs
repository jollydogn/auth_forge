using AuthForge.Authentication;
using AuthForge.Clients;
using AuthForge.Configuration;
using AuthForge.Managers.Auth;
using AuthForge.Managers.Groups;
using AuthForge.Managers.Permissions;
using AuthForge.Managers.Roles;
using AuthForge.Managers.Users;
using AuthForge.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthForge.Extensions;

public static class AuthForgeServiceCollectionExtensions
{
    /// <summary>
    /// AuthForge bileşenlerini, DI bağımlılıklarını ve JWT Bearer tabanlı 
    /// kimlik doğrulamayı projenize ekler.
    /// </summary>
    public static IServiceCollection AddAuthForge(
        this IServiceCollection services,
        IConfiguration configuration,
        string authOptionsSectionName = "AuthForge",
        string adminOptionsSectionName = "AuthForgeAdmin",
        Action<AuthForgeOptions>? authSetupAction = null,
        Action<AuthForgeAdminOptions>? adminSetupAction = null)
    {
        // 1. Core Servis Bağlamaları
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddHttpClient();

        // 2. Options Pattern Konfigürasyonları
        var authOptionsSection = configuration.GetSection(authOptionsSectionName);
        var adminOptionsSection = configuration.GetSection(adminOptionsSectionName);

        services.Configure<AuthForgeOptions>(authOptionsSection);
        services.Configure<AuthForgeAdminOptions>(adminOptionsSection);

        if (authSetupAction != null)
        {
            services.Configure(authSetupAction);
        }

        if (adminSetupAction != null)
        {
            services.Configure(adminSetupAction);
        }

        // Configuration değerlerini anlık olarak al (JWT konfigürasyonu için gerekli)
        var authOptions = new AuthForgeOptions();
        authOptionsSection.Bind(authOptions);
        authSetupAction?.Invoke(authOptions);

        // 3. AuthForge Manager ve İstemcileri
        services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IAuthForgeAdminClient, AuthForgeAdminClient>();
        
        services.AddScoped<IAuthManager, AuthManager>();
        services.AddScoped<IUsersManager, UsersManager>();
        services.AddScoped<IRolesManager, RolesManager>();
        services.AddScoped<IGroupsManager, GroupsManager>();
        services.AddScoped<IPermissionsManager, PermissionsManager>();

        // 4. JWT Kimlik Doğrulama Katmanı
        if (!string.IsNullOrEmpty(authOptions.Authority))
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = authOptions.Authority;
                options.Audience = authOptions.Audience;
                options.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;

                // Token validasyon kuralları
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authOptions.Authority,
                    ValidateAudience = true,
                    ValidAudience = authOptions.Audience,
                    ValidateLifetime = true,
                    RoleClaimType = authOptions.RoleClaimType,
                    NameClaimType = authOptions.NameClaimType,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        return services;
    }

    /// <summary>
    /// AuthForge için gerekli olan Authentication ve Authorization middleware'lerini 
    /// HTTP pipeline'a ekler.
    /// </summary>
    public static IApplicationBuilder UseAuthForge(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
