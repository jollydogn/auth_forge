using System;
using System.Linq;
using System.Reflection;
using AuthForge.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthForge.Extensions;

/// <summary>
/// Extension methods for registering the AuthForge Permission Definition system.
/// </summary>
public static class AuthForgePermissionExtensions
{
    /// <summary>
    /// Scans the given assemblies for all classes implementing AuthForgePermissionDefinitionProvider,
    /// registers them into DI, sets up the PermissionDefinitionManager, and registers the
    /// data seeder that auto-seeds permissions into the database on application startup.
    /// 
    /// Usage in Program.cs:
    /// <code>
    /// builder.Services.AddAuthForgePermissions&lt;AppDbContext&gt;(typeof(Program).Assembly);
    /// </code>
    /// </summary>
    /// <typeparam name="TDbContext">The consumer's DbContext type (must have ConfigureAuthForge called)</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="assemblies">Assemblies to scan for PermissionDefinitionProviders</param>
    public static IServiceCollection AddAuthForgePermissions<TDbContext>(
        this IServiceCollection services,
        params Assembly[] assemblies)
        where TDbContext : DbContext
    {
        // 1. Scan and register all PermissionDefinitionProviders
        var providerType = typeof(AuthForgePermissionDefinitionProvider);

        foreach (var assembly in assemblies)
        {
            var providerTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && providerType.IsAssignableFrom(t));

            foreach (var type in providerTypes)
            {
                services.AddSingleton(providerType, type);
            }
        }

        // 2. Register the PermissionDefinitionManager (runtime registry)
        services.AddSingleton<IAuthForgePermissionDefinitionManager, AuthForgePermissionDefinitionManager>();

        // 3. Register the data seeder as a hosted service (auto-seeds on startup)
        services.AddHostedService<AuthForgePermissionDataSeeder<TDbContext>>();

        return services;
    }
}
