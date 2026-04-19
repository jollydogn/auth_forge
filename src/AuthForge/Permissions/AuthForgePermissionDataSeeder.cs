using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthForge.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthForge.Permissions;

/// <summary>
/// Hosted service that runs on application startup.
/// Discovers all PermissionDefinitionProviders, collects their definitions,
/// and seeds them into the database automatically (insert-if-not-exists).
/// </summary>
public class AuthForgePermissionDataSeeder<TDbContext> : IHostedService
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuthForgePermissionDataSeeder<TDbContext>> _logger;

    public AuthForgePermissionDataSeeder(
        IServiceProvider serviceProvider,
        ILogger<AuthForgePermissionDataSeeder<TDbContext>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var manager = scope.ServiceProvider.GetRequiredService<IAuthForgePermissionDefinitionManager>();

        _logger.LogInformation("AuthForge: Starting permission data seed...");

        var groups = manager.GetGroups();
        if (!groups.Any())
        {
            _logger.LogInformation("AuthForge: No permission definition providers found. Skipping seed.");
            return;
        }

        foreach (var groupDef in groups)
        {
            // Upsert group
            var existingGroup = await dbContext.Set<AuthForgePermissionGroup>()
                .FirstOrDefaultAsync(g => g.Name == groupDef.Name, cancellationToken);

            if (existingGroup == null)
            {
                existingGroup = new AuthForgePermissionGroup
                {
                    Id = Guid.NewGuid(),
                    Name = groupDef.Name,
                    DisplayName = groupDef.DisplayName
                };
                await dbContext.Set<AuthForgePermissionGroup>().AddAsync(existingGroup, cancellationToken);
                _logger.LogInformation("AuthForge: Created permission group '{Group}'", groupDef.Name);
            }
            else
            {
                existingGroup.DisplayName = groupDef.DisplayName;
            }

            // Seed permissions recursively
            foreach (var permDef in groupDef.Permissions)
            {
                await SeedPermissionRecursiveAsync(dbContext, existingGroup, permDef, parentId: null, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("AuthForge: Permission data seed completed successfully.");
    }

    private async Task SeedPermissionRecursiveAsync(
        TDbContext dbContext,
        AuthForgePermissionGroup group,
        AuthForgePermissionDefinition permDef,
        Guid? parentId,
        CancellationToken cancellationToken)
    {
        var existing = await dbContext.Set<AuthForgePermission>()
            .FirstOrDefaultAsync(p => p.Name == permDef.Name, cancellationToken);

        Guid currentId;

        if (existing == null)
        {
            currentId = Guid.NewGuid();
            var entity = new AuthForgePermission
            {
                Id = currentId,
                Name = permDef.Name,
                DisplayName = permDef.DisplayName,
                GroupId = group.Id,
                ParentPermissionId = parentId
            };
            await dbContext.Set<AuthForgePermission>().AddAsync(entity, cancellationToken);
            _logger.LogInformation("AuthForge: Created permission '{Permission}' in group '{Group}'",
                permDef.Name, group.Name);
        }
        else
        {
            currentId = existing.Id;
            existing.DisplayName = permDef.DisplayName;
            existing.GroupId = group.Id;
            existing.ParentPermissionId = parentId;
        }

        foreach (var childDef in permDef.Children)
        {
            await SeedPermissionRecursiveAsync(dbContext, group, childDef, currentId, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
