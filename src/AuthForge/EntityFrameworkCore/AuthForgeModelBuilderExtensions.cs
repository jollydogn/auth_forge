using AuthForge.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthForge.EntityFrameworkCore;

/// <summary>
/// EntityFrameworkCore configuration extensions for the AuthForge module.
/// Consumers call this method within their DbContext.OnModelCreating to include
/// AuthForge tables (Users, Roles, Groups, Permissions, Grants) in their migrations.
/// </summary>
public static class AuthForgeModelBuilderExtensions
{
    /// <summary>
    /// Configures the EF Core Model Builder to include all AuthForge tables.
    /// </summary>
    /// <param name="builder">EF Core Model Builder</param>
    /// <param name="tablePrefix">Table prefix for generated tables (e.g., 'AuthForge_Users')</param>
    /// <param name="schema">Optional database schema</param>
    public static void ConfigureAuthForge(this ModelBuilder builder, string tablePrefix = "AuthForge_", string? schema = null)
    {
        // ── User ──
        builder.Entity<AuthForgeUser>(b =>
        {
            b.ToTable(tablePrefix + "Users", schema);
            b.HasKey(x => x.Id);
            b.Property(x => x.Username).IsRequired().HasMaxLength(256);
            b.Property(x => x.Email).IsRequired().HasMaxLength(256);
            b.Property(x => x.FirstName).HasMaxLength(128);
            b.Property(x => x.LastName).HasMaxLength(128);
        });

        // ── Role ──
        builder.Entity<AuthForgeRole>(b =>
        {
            b.ToTable(tablePrefix + "Roles", schema);
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);
            b.Property(x => x.Description).HasMaxLength(500);
        });

        // ── Group ──
        builder.Entity<AuthForgeGroup>(b =>
        {
            b.ToTable(tablePrefix + "Groups", schema);
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);
            b.Property(x => x.Path).IsRequired().HasMaxLength(500);
        });

        // ── UserRole (Many-to-Many) ──
        builder.Entity<AuthForgeUserRole>(b =>
        {
            b.ToTable(tablePrefix + "UserRoles", schema);
            b.HasKey(x => new { x.UserId, x.RoleId });

            b.HasOne(x => x.User)
             .WithMany(x => x.UserRoles)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Role)
             .WithMany(x => x.UserRoles)
             .HasForeignKey(x => x.RoleId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── UserGroup (Many-to-Many) ──
        builder.Entity<AuthForgeUserGroup>(b =>
        {
            b.ToTable(tablePrefix + "UserGroups", schema);
            b.HasKey(x => new { x.UserId, x.GroupId });

            b.HasOne(x => x.User)
             .WithMany(x => x.UserGroups)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Group)
             .WithMany(x => x.UserGroups)
             .HasForeignKey(x => x.GroupId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── PermissionGroup ──
        builder.Entity<AuthForgePermissionGroup>(b =>
        {
            b.ToTable(tablePrefix + "PermissionGroups", schema);
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);
            b.HasIndex(x => x.Name).IsUnique();
            b.Property(x => x.DisplayName).HasMaxLength(256);
        });

        // ── Permission (with self-referencing hierarchy) ──
        builder.Entity<AuthForgePermission>(b =>
        {
            b.ToTable(tablePrefix + "Permissions", schema);
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);
            b.HasIndex(x => x.Name).IsUnique();
            b.Property(x => x.DisplayName).HasMaxLength(256);

            b.HasOne(x => x.Group)
             .WithMany(x => x.Permissions)
             .HasForeignKey(x => x.GroupId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.ParentPermission)
             .WithMany(x => x.Children)
             .HasForeignKey(x => x.ParentPermissionId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);
        });

        // ── PermissionGrant (Role or User -> Permission) ──
        builder.Entity<AuthForgePermissionGrant>(b =>
        {
            b.ToTable(tablePrefix + "PermissionGrants", schema);
            b.HasKey(x => x.Id);
            b.Property(x => x.ProviderName).IsRequired().HasMaxLength(64);
            b.Property(x => x.ProviderKey).IsRequired().HasMaxLength(256);
            b.HasIndex(x => new { x.PermissionId, x.ProviderName, x.ProviderKey }).IsUnique();

            b.HasOne(x => x.Permission)
             .WithMany(x => x.Grants)
             .HasForeignKey(x => x.PermissionId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
